using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;
using Abstractions.Repositories;
using Domain.Entities;

namespace Persistence.Repositories
{
	public class AuthorRepository : RepositoryBase<Author>, IAuthorRepository
	{
		public AuthorRepository(LibraryDbContext context) : base(context)
		{
			
		}
		protected override IQueryable<Author> ApplySort(IQueryable<Author> source,PagedQuery query)
		{
			var descending = query.SortDirection == SortDirection.Descending;

			return query.SortBy?.ToLowerInvariant() switch
			{
				"firsname" => descending ? source.OrderByDescending(a => a.FirstName) : source.OrderBy(a => a.FirstName),
				"lastname" => descending ? source.OrderByDescending(a => a.LastName) : source.OrderBy(a => a.LastName),
				_ => source.OrderByDescending(a => a.LastName)
			};
		}
	}
}
