using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
namespace Abstractions.Repositories
{
	public interface IReservationRepository : IRepositoryBase<Reservation>
	{
		Task<Reservation?> GetOldestPendingForBookAsync(Guid bookId, CancellationToken cancellationToken = default);
		Task<Reservation?> GetFulfilledForMemberAndBookAsync(Guid memberId, Guid bookId, CancellationToken cancellationToken = default);
		Task<List<Reservation>> GetExpiredFulfilledAsync(DateTime asOf, CancellationToken cancellationToken = default);

	}
}
