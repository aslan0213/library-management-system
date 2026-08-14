using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;
using Abstractions.Specifications;
namespace Abstractions.Services
{
	public interface IBookService
	{
		Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<PagedResult<Book>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
		Task<Book> CreateAsync(Book book, List<Guid> categoryIds, CancellationToken cancellationToken = default);
		Task UpdateAsync(Book book, List<Guid> categoryIds, CancellationToken cancellationToken = default);
		Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
		Task<PagedResult<Book>> SearchAsync(string? title, Guid? authorId, Guid? publisherId, Guid? categoryId, int? minYear, int? maxYear, bool? onlyAvailable, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
	}
}
