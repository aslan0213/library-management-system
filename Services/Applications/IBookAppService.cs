using Shared.Dtos.Book;
using Shared.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public interface IBookAppService
	{
		Task<BookResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<PagedResult<BookResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
		Task<BookResponse> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default);
		Task UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken cancellationToken = default);
		Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
	}
}
