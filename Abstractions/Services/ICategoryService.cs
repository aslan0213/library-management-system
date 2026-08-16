using Abstractions.Paging;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions.Services
{
	public interface ICategoryService
	{
		Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
		Task<PagedResult<Category>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
		Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default);
		Task UpdateAsync(Category category, CancellationToken cancellationToken = default);
		Task DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default);
	}
}
