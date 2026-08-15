using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;
using Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using Abstractions.Specifications;
using Persistence.Specifications;
namespace Persistence.Repositories
{
	public class BookRepository : RepositoryBase<Book>, IBookRepository
	{
		public BookRepository(LibraryDbContext context) : base(context)
		{
			
		}
		protected override IQueryable<Book> IncludeRelated(IQueryable<Book> source)
		{
			return source.Include(b=> b.Author).Include(b => b.Categories).Include(b => b.Publisher);
		}
		protected override IQueryable<Book> ApplySort(IQueryable<Book> source, PagedQuery query)
		{
			var descending = query.SortDirection == SortDirection.Descending;
			return query.SortBy?.ToLowerInvariant() switch
			{
				"title" => descending ? source.OrderByDescending(b => b.Title) : source.OrderBy(b => b.Title),
				"publishedyear" => descending ? source.OrderByDescending(b => b.PublishedYear) : source.OrderBy(b => b.PublishedYear),
				"publisher" => descending ? source.OrderByDescending(b => b.Publisher) : source.OrderBy(b => b.Publisher),
				"totalcopies" => descending ? source.OrderByDescending(b => b.TotalCopies) : source.OrderBy(b => b.TotalCopies),
				"availablecopies" => descending ? source.OrderByDescending(b => b.AvailableCopies) : source.OrderBy(b => b.AvailableCopies),
				_ => source.OrderBy(b => b.Title)
			};

		}

		public async Task<PagedResult<Book>> SearchAsync(ISpecification<Book> specification, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
		{
			var query = SpecificationEvaluator.Apply(FindAll(trackChanges: false), specification);
			var totalCount = await query.CountAsync(cancellationToken);
			var items = await query
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync(cancellationToken);

			return new PagedResult<Book>
			{
				Items = items,
				TotalCount = totalCount,
				PageNumber = pageNumber,
				PageSize = pageSize
			};
		}
	}
}
