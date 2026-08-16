using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;
using Abstractions.Repositories;
using Abstractions.Services;
using Domain.Entities;
using Domain.Exceptions;

namespace Services.Reservations
{
	public class ReservationService : IReservationService
	{
		private readonly IUnitOfWork _unitOfWork;
		public ReservationService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
			await _unitOfWork.Reservations.GetByIdAsync(id, cancellationToken);

		public async Task<PagedResult<Reservation>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.Reservations.GetPagedAsync(query, cancellationToken);
		}

		public async Task<Reservation> CreateReservationAsync(Guid bookId, Guid memberId, CancellationToken cancellationToken = default)
		{
			var book = await _unitOfWork.Books.GetByIdAsync(bookId, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Book), bookId);
			var member = await _unitOfWork.Members.GetByIdAsync(memberId, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Member), memberId);

			if (book.AvailableCopies > 0) 
			{
				throw new BusinessRuleViolationException($"'{book.Title}' has available copies — borrow it directly instead of reserving.");
			}

			//check for existing active reservation 
			var existingActive  = await _unitOfWork.Reservations.GetActiveForMemberAndBookAsync(memberId, bookId, cancellationToken);
			if(existingActive is not null)
			{
				throw new BusinessRuleViolationException($"You already have an active reservation for '{book.Title}'.");
			}
			//.

			await _unitOfWork.BeginTransactionAsync(cancellationToken);
			try
			{
				var reservation = new Reservation
				{
					Id = Guid.NewGuid(),
					BookId = book.Id,
					MemberId = member.Id,
					Book = book,
					Member = member,
					ReservedAt = DateTime.UtcNow,
					Status = ReservationStatus.Pending
				};
				await _unitOfWork.Reservations.AddAsync(reservation, cancellationToken);
				await _unitOfWork.SaveChangesAsync(cancellationToken);

				var notification = new Notification
				{
					Id = Guid.NewGuid(),
					MemberId = member.Id,
					Message = $"Your reservation for '{book.Title}' has been created. You'll be notified when it's available.",
					SentAt = DateTime.UtcNow,
					IsRead = false
				};
				await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
				await _unitOfWork.SaveChangesAsync(cancellationToken);

				await _unitOfWork.CommitTransactionAsync(cancellationToken);
				return reservation;
			}
			catch
			{
				await _unitOfWork.RollbackTransactionAsync(cancellationToken);
				throw;
			}
		}

		public async Task CancelReservationAsync(Guid reservationId, CancellationToken cancellationToken = default)
		{
			var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Reservation), reservationId);
			if(reservation.Status != ReservationStatus.Pending && reservation.Status != ReservationStatus.Fullfilled)
			{
				throw new BusinessRuleViolationException("Only pending or fulfilled reservations can be canceled.");
			}
			var wasFulfilled = reservation.Status == ReservationStatus.Fullfilled;
			reservation.Status = ReservationStatus.Canceled;
			reservation.HeldUntil = null;
			_unitOfWork.Reservations.Update(reservation);
			if (wasFulfilled)
			{
				await FulfillNextOrReleaseAsync(reservation.BookId, cancellationToken);
			}
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		} 

		public async Task FulfillNextOrReleaseAsync(Guid bookId, CancellationToken cancellationToken = default)
		{
			var nextPending = await _unitOfWork.Reservations.GetOldestPendingForBookAsync(bookId, cancellationToken);
			if(nextPending != null)
			{
				nextPending.Status = ReservationStatus.Fullfilled;
				nextPending.HeldUntil = DateTime.UtcNow.AddHours(48); // Hold for 48 hours
				_unitOfWork.Reservations.Update(nextPending);

				await _unitOfWork.Notifications.AddAsync(new Notification
				{
					Id = Guid.NewGuid(),
					MemberId = nextPending.MemberId,
					Message = $"Your reservation for '{nextPending.Book.Title}' is now available. Please pick it up within 48 hours.",
					SentAt = DateTime.UtcNow,
					IsRead = false
				}, cancellationToken);
			}
			else
			{
				var book = await _unitOfWork.Books.GetByIdAsync(bookId, cancellationToken);
				if(book is not null)
				{
					book.AvailableCopies++;
					_unitOfWork.Books.Update(book);
				}
			}
		}
	}
}
