using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Abstractions.Paging;
using Abstractions.Repositories;
namespace Persistence.Repositories
{
	public class MemberRepository : RepositoryBase<Member>, IMemberRepository
	{
		public MemberRepository(LibraryDbContext context) : base(context)
		{

		}
		protected override IQueryable<Member> ApplySort(IQueryable<Member> source, PagedQuery query)
		{
			var descending = query.SortDirection == SortDirection.Descending;
			return query.SortBy?.ToLowerInvariant() switch
			{
				"firstname" => descending ? source.OrderByDescending(m => m.FirstName) : source.OrderBy(m => m.FirstName),
				"lastname" => descending ? source.OrderByDescending(m => m.LastName) : source.OrderBy(m => m.LastName),
				_ => source.OrderBy(m => m.LastName)
			};
		}
	}
}
