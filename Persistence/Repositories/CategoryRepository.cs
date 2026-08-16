using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
namespace Persistence.Repositories
{
	public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
	{
		public CategoryRepository(LibraryDbContext context) : base(context)
		{
		}

		public async Task<List<Category>> GetByIdsTrackedAsync(List<Guid> ids, CancellationToken cancellationToken = default)
		{
			return await _context.Categories
				.Where(c => ids.Contains(c.Id))
				.ToListAsync(cancellationToken);
		}
	}
}
