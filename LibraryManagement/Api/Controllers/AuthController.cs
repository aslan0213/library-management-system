using Microsoft.AspNetCore.Mvc;
using Abstractions.Services;
using Shared.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;


namespace LibraryManagement.Api.Controllers
{
	/// <summary>
	/// Handles registration, login, token refresh, and logout.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly IValidator<RegisterRequest> _registerValidator;
		private readonly IValidator<LoginRequest> _loginValidator;
		public AuthController(IAuthService authorService, IValidator<LoginRequest> loginValidator, IValidator<RegisterRequest> registerValidator)
		{
			_authService = authorService;
			_loginValidator = loginValidator;
			_registerValidator = registerValidator;
		}
		/// <summary>
		/// Registers a new user with the default User role.
		/// </summary>
		[HttpPost("register")]
		[ProducesResponseType(typeof(AuthResponse),StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
		{
			await _registerValidator.ValidateAndThrowAsync(request, cancellationToken);
			var result = await _authService.RegisterAsync(request.Email, request.Password, cancellationToken);
			return StatusCode(StatusCodes.Status201Created, ToAuthResponse(result));
		}

		/// <summary>
		/// Authenticates a user and returns an access and refresh token.
		/// </summary>
		[HttpPost("login")]
		[ProducesResponseType(typeof(AuthResponse),StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
		{
			await _loginValidator.ValidateAndThrowAsync(request, cancellationToken);
			var result = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);
			return Ok(ToAuthResponse(result));
		}


		/// <summary>
		/// Exchanges a valid refresh token for a new access and refresh token pair.
		/// </summary>
		[HttpPost("refresh")]
		[ProducesResponseType(typeof(TokenResponse),StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<TokenResponse>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
		{
			var result = await _authService.RefreshAsync(request.RefreshToken, cancellationToken);
			return Ok(new TokenResponse
			{
				AccessToken = result.AccessToken,
				RefreshToken = result.RefreshToken,
				ExpiresAt = result.ExpiredAt
			});
		}

		/// <summary>
		/// Revokes the given refresh token, ending the session.
		/// </summary>
		[HttpPost("logout")]
		[Authorize]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
		{
			await _authService.LogoutAsync(request.RefreshToken, cancellationToken);
			return NoContent();
		}

		/// <summary>
		/// Promotes a user to the Admin role. Admin-only.
		/// </summary>
		[HttpPut("users/{userId:guid}/promote")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> PromoteToAdmin([FromBody] Guid userId, CancellationToken cancellationToken)
		{
			await _authService.PromoteToAdminAsync(userId, cancellationToken);
			return NoContent();
		}

		private static AuthResponse ToAuthResponse(AuthResult result)
		{
			return new()
			{
				UserId = result.User.Id,
				Email = result.User.Email,
				Role = result.User.role.ToString(),
				AccessToken = result.AccessToken,
				RefreshToken = result.RefreshToken,
				ExpiresAt = result.ExpiredAt
			};
		}
	} 
}
