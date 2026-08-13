using Domain.Entities;
using Shared.Dtos.Category;
using Shared.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public interface ICategoryAppService
	{
		Task<CategoryResponse?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
		Task<PagedResult<CategoryResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
		Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
		Task UpdateAsync(Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
		Task DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default);
	}
}
