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
			return source.Include(r => r.Book).Include(r => r.Member);
		}
	}
}
