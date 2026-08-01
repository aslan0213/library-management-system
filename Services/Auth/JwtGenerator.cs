using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Abstractions.Services;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Services.Auth
{
	public class JwtGenerator : IJwtTokenGenerator
	{
		private readonly JwtSettings _jwtSettings;
		public JwtGenerator(IOptions<JwtSettings> jwtSettings)
		{
			_jwtSettings = jwtSettings.Value;
		}
		public (string token, DateTime ExpiredAt) GenerateAccessToken(User user)
		{
			var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes);
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(ClaimTypes.Role, user.role.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};
			var key = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.Secret));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var token = new JwtSecurityToken(
				issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				claims: claims,
				expires: expiresAt,
				signingCredentials: credentials
			);
			return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
		}

		public string GenerateRefreshToken()
		{
			var bytes = new byte[64];
			RandomNumberGenerator.Fill(bytes);
			return Convert.ToBase64String(bytes);
		}
	}
}
