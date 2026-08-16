using Abstractions.Paging;
using Abstractions.Services;
using Abstractions.Repositories;
using Domain.Entities;
using Domain.Exceptions;

namespace Services
{
	public class LoanService : ILoanService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IReservationService _reservationService;
		public LoanService(IUnitOfWork unitOfWork, IReservationService reservationService)
		{
			_unitOfWork = unitOfWork;
			_reservationService = reservationService;
		}
		public async Task<Loan> CreateLoanAsync(Guid bookId, Guid memberId, DateTime dueAt, CancellationToken cancellationToken = default)
		{
			var book = await _unitOfWork.Books.GetByIdAsync(bookId, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Book), bookId);
			var member = await _unitOfWork.Members.GetByIdAsync(memberId, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Member), memberId);


			if (book.AvailableCopies <= 0)
			{
				var heldReservations = await _unitOfWork.Reservations.GetFulfilledForMemberAndBookAsync(memberId,  bookId, cancellationToken);
				if (heldReservations is null || heldReservations.HeldUntil < DateTime.UtcNow) 
				{
					throw new BusinessRuleViolationException($"No available copies for book '{book.Title}'.");
				}
				heldReservations.Status = ReservationStatus.Completed;
				heldReservations.HeldUntil = null;
				_unitOfWork.Reservations.Update(heldReservations);
			}
			else
			{
				book.AvailableCopies--;
				_unitOfWork.Books.Update(book);
			}



			var loan = new Loan
			{
				Id = Guid.NewGuid(),
				BookId = bookId,
				MemberId = memberId,
				Book = book,
				Member = member,
				BorrowedAt = DateTime.UtcNow,
				DueAt = dueAt,
				ReturnedAt = null
			};
			await _unitOfWork.Loans.AddAsync(loan, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return loan;
		}

		public async Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)=>
			await _unitOfWork.Loans.GetByIdAsync(id, cancellationToken);



		public async Task<PagedResult<Loan>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)=>
			await _unitOfWork.Loans.GetPagedAsync(query, cancellationToken);

		public async Task ReturnLoanAsync(Guid loanId, CancellationToken cancellationToken = default)
		{
			var loan = await _unitOfWork.Loans.GetByIdAsync(loanId, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Loan), loanId);

			if(loan.ReturnedAt is not null)
				throw new BusinessRuleViolationException($"Loan '{loanId}' has already been returned.");
			var book = await _unitOfWork.Books.GetByIdAsync(loan.BookId, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Book), loan.BookId);
			loan.ReturnedAt = DateTime.UtcNow;
			_unitOfWork.Loans.Update(loan);
			await _reservationService.FulfillNextOrReleaseAsync(loan.BookId, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
	}
}
