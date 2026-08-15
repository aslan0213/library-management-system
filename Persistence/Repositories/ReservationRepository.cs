using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
namespace Persistence.Repositories
{
	public class ReservationRepository : RepositoryBase<Reservation>, IReservationRepository
	{
		public ReservationRepository(LibraryDbContext context) : base(context)
		{
		}

		protected override IQueryable<Reservation> IncludeRelated(IQueryable<Reservation> source)
		{
			return source
				.Include(r => r.Book).ThenInclude(b => b.Author)
				.Include(r => r.Book).ThenInclude(b => b.Publisher)
				.Include(r => r.Book).ThenInclude(b => b.Categories)
				.Include(r => r.Member);
		}

		public async Task<List<Reservation>> GetExpiredFulfilledAsync(DateTime asOf, CancellationToken cancellationToken = default)
		{
			return await FindAll(trackChanges: true)
				.Where(r => r.Status == ReservationStatus.Fullfilled && r.HeldUntil!=null && r.HeldUntil.Value < asOf)
				.ToListAsync(cancellationToken);
		}

		public async Task<Reservation?> GetFulfilledForMemberAndBookAsync(Guid memberId, Guid bookId, CancellationToken cancellationToken = default)
		{
			return await FindAll(trackChanges: true)
				.FirstOrDefaultAsync(r =>
				r.MemberId == memberId &&
				r.BookId == bookId &&
				r.Status == ReservationStatus.Fullfilled,
				cancellationToken);
		}

		public async Task<Reservation?> GetOldestPendingForBookAsync(Guid bookId, CancellationToken cancellationToken = default)
		{
			return await FindAll(trackChanges: true)
				.Where(r => r.Status == ReservationStatus.Pending && r.BookId == bookId)
				.OrderBy(r => r.ReservedAt)
				.FirstOrDefaultAsync(cancellationToken);
		}

		public async Task<Reservation?> GetActiveForMemberAndBookAsync(Guid memberId, Guid bookId, CancellationToken cancellationToken = default)
		{
			return await FindAll(trackChanges: false)
				.FirstOrDefaultAsync(r =>
				r.MemberId == memberId &&
				r.BookId == bookId &&
				(r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Fullfilled),
				cancellationToken);
		}
	}
}
