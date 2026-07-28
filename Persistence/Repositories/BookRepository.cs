using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;
using Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
namespace Persistence.Repositories
{
	public class BookRepository : RepositoryBase<Book>,IBookRepository
	{
		public BookRepository(LibraryDbContext context) : base(context)
		{
			
		}
		protected override IQueryable<Book> IncludeRelated(IQueryable<Book> source)
		{
			return source.Include(b=> b.Author);
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


	}
}
