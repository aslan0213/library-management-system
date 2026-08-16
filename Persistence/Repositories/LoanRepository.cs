using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Abstractions.Paging;

namespace Persistence.Repositories
{
	public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
	{
		public LoanRepository(LibraryDbContext context) : base(context)
		{
			
		}
		protected override IQueryable<Loan> IncludeRelated(IQueryable<Loan> source)
		{
			return source
				.Include(l => l.Book).ThenInclude(b => b.Author)
				.Include(l => l.Book).ThenInclude(b => b.Publisher)
				.Include(l => l.Book).ThenInclude(b => b.Categories)
				.Include(l => l.Member);
		}
		protected override IQueryable<Loan> ApplySort(IQueryable<Loan> source, PagedQuery query)
		{
			var descending = query.SortDirection == SortDirection.Descending;
			return query.SortBy?.ToLowerInvariant() switch
			{
				"borrowedat" => descending ? source.OrderByDescending(l => l.BorrowedAt) : source.OrderBy(l => l.BorrowedAt),
				"dueat" => descending ? source.OrderByDescending(l => l.DueAt) : source.OrderBy(l => l.DueAt),
				_ => source.OrderByDescending(l => l.BorrowedAt)
			};
		}
	}
}
