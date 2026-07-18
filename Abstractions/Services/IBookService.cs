using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;
namespace Abstractions.Services
{
	public interface IBookService
	{
		Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<PagedResult<Book>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
		Task<Book> CreateAsync(Book book, CancellationToken cancellationToken = default);
		Task<Book> UpdateAsync(Book book, CancellationToken cancellationToken = default);
		Task<Book> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
	}
}
