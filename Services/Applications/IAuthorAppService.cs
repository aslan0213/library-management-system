using Shared.Dtos.Author;
using Shared.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public interface IAuthorAppService
	{
		Task<AuthorResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<PagedResult<AuthorResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
		Task<AuthorResponse> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default);
		Task UpdateAsync(Guid id, UpdateAuthorRequest request, CancellationToken cancellationToken = default);
		Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
	}
}
