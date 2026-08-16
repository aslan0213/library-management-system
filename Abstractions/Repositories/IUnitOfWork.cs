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
		INotificationRepository Notifications { get; }
		IReservationRepository Reservations { get; }
		ICategoryRepository Categories { get; }
		IPublisherRepository Publishers { get; }
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
		Task BeginTransactionAsync(CancellationToken cancellationToken = default);
		Task CommitTransactionAsync(CancellationToken cancellationToken = default);
		Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
	}
}
