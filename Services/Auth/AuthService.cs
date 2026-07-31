using System.Text;
using Abstractions.Repositories;
using Abstractions.Services;
using Microsoft.Extensions.Options;
using Domain.Exceptions;
using Domain.Entities;
using System.Security.Cryptography;

namespace Services.Auth
{
	public class AuthService : IAuthService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IJwtTokenGenerator _jwtTokenGenerator;
		private readonly IPasswordHasher _passwordHasher;
		private readonly JwtSettings _jwtSettings;
		public AuthService(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator, IPasswordHasher passwordHasher, IOptions<JwtSettings> jwtSettings)
		{
			_unitOfWork = unitOfWork;
			_jwtTokenGenerator = jwtTokenGenerator;
			_passwordHasher = passwordHasher;
			_jwtSettings = jwtSettings.Value;
		}
		public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
		{
			var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
			if (user == null || !_passwordHasher.Verify(password, user.PasswordHash))
			{ 
				throw new InvalidCredentialException();
			}
			return await IssueTokenAsync(user, cancellationToken);
		}

		public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
		{
			var tokenHash = HashToken(refreshToken);
			var existing = await _unitOfWork.RefreshTokens.GetByTokenHashAsync(tokenHash, cancellationToken);
			if (existing is not null && existing.IsActive)
			{
				existing.RevokedAt = DateTime.UtcNow;
				await _unitOfWork.SaveChangesAsync(cancellationToken);
			}
		}

		public async Task PromoteToAdminAsync(Guid userId, CancellationToken cancellationToken = default)
		{
			var user = await _unitOfWork.Users.GetByIdAsync(userId,cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(User), userId);
			user.role = Role.Admin;
			_unitOfWork.Users.Update(user);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		public async Task<TokenResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
		{
			var tokenHash = HashToken(refreshToken);
			var existing = await _unitOfWork.RefreshTokens.GetByTokenHashAsync(tokenHash, cancellationToken);
			if (existing == null || !existing.IsActive)
			{
				throw new InvalidRefreshTokenException();
			}
			existing.RevokedAt = DateTime.UtcNow;
			var user = await _unitOfWork.Users.GetByIdAsync(existing.UserId, cancellationToken)
				?? throw new InvalidRefreshTokenException();
			var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user);
			var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
			await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
			{
				Id = Guid.NewGuid(),
				TokenHash = HashToken(newRefreshToken),
				UserId = user.Id,
				ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
				CreatedAt = DateTime.UtcNow
			}, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return new TokenResult
			{
				AccessToken = accessToken,
				RefreshToken = newRefreshToken,
				ExpiredAt = expiresAt
			};
		}

		public async Task<AuthResult> RegisterAsync(string email, string password, CancellationToken cancellationToken = default)
		{
			var existing = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
			if (existing != null)
			{
				throw new EmailAlreadyRegisteredException(email);
			}
			var user = new User
			{
				Id = Guid.NewGuid(),
				Email = email,
				PasswordHash = _passwordHasher.Hash(password),
				role = Role.User,
				CreatedAt = DateTime.UtcNow
			};
			await _unitOfWork.Users.AddAsync(user, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return await IssueTokenAsync(user, cancellationToken);	
		}
		private async Task<AuthResult> IssueTokenAsync(User user, CancellationToken cancellationToken = default)
		{
			var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user);
			var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
			await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
			{
				Id = Guid.NewGuid(),
				TokenHash = HashToken(refreshToken),
				UserId = user.Id,
				ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
				CreatedAt = DateTime.UtcNow
			}, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return new AuthResult
			{
				User = user,
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				ExpiredAt = expiresAt
			};
		}
		private static string HashToken(string token)
		{
			var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
			return Convert.ToBase64String(bytes);
		}
	}
}
