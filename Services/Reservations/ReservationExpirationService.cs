using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Repositories;
using Abstractions.Services;
using Domain.Entities;

namespace Services.Reservations
{
	public class ReservationExpirationService : IReservationExpirationService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IReservationService _reservationService;

		public ReservationExpirationService(IUnitOfWork unitOfWork, IReservationService reservationService)
		{
			_unitOfWork = unitOfWork;
			_reservationService = reservationService;
		}
		public async Task ProcessExpiredReservationsAsync(CancellationToken cancellationToken = default)
		{
			var expired = await _unitOfWork.Reservations.GetExpiredFulfilledAsync(DateTime.UtcNow, cancellationToken);
			foreach (var reservation in expired)
			{
				await _unitOfWork.BeginTransactionAsync(cancellationToken);
				try
				{
					reservation.Status = ReservationStatus.Expired;
					_unitOfWork.Reservations.Update(reservation);
					await _unitOfWork.SaveChangesAsync(cancellationToken);

					await _reservationService.FulfillNextOrReleaseAsync(reservation.BookId, cancellationToken);
					await _unitOfWork.SaveChangesAsync(cancellationToken);

					await _unitOfWork.CommitTransactionAsync(cancellationToken);

				}
				catch
				{
					await _unitOfWork.RollbackTransactionAsync(cancellationToken);
					throw;

				}
			}
		}
	}
}
