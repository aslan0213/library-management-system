using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions.Repositories
{
	public interface IUnitOfWork
	{
		IAuthorRepository Authors { get; }
		IBookRepository Books { get; }
		ILoanRepository Loans { get; }
		IMemberRepository Members { get; }
		IUserRepository Users { get; }
		IRefreshTokenRepository RefreshTokens { get; }
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}
