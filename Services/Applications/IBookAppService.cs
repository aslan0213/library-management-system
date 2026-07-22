using Shared.Dtos.Book;
using Shared.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public interface IBookAppService
	{
		Task<BookResponce?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<PagedResult<BookResponce>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
		Task<BookResponce> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default);
		Task UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken cancellationToken = default);
		Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
	}
}
