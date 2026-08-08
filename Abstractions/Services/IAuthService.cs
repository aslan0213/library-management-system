using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions.Services
{
	public interface IAuthService
	{
		Task<AuthResult> RegisterAsync(string email, string password, CancellationToken cancellationToken = default);
		Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
		Task<TokenResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
		Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
		Task PromoteToAdminAsync(Guid userId, CancellationToken cancellationToken = default);
	}
}
