using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions.Services
{
	public interface IReservationService
	{
		Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<Reservation> CreateReservationAsync(Guid bookId, Guid memberId, CancellationToken cancellationToken = default);
		Task CancelReservationAsync(Guid reservationId, CancellationToken cancellationToken = default);
		Task FulfillNextOrReleaseAsync(Guid bookId, CancellationToken cancellationToken = default);
	}
}
