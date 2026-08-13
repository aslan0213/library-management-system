using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Shared.Dtos.Reservation;
using Shared.Paging;
namespace Services.Applications
{
	public interface IReservationAppService
	{
		Task<ReservationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<ReservationResponse> CreateReservationAsync(CreateReservationRequest request, CancellationToken cancellationToken = default);
		Task CancelReservationAsync(Guid reservationId, CancellationToken cancellationToken = default);
	}
}
