using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;
namespace Abstractions.Services
{
	public interface IAuthorService
	{
		Task<Author?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default); 
		Task<PagedResult<Author>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
		Task<Author> CreateAsync(Author author, CancellationToken cancellationToken = default);
		Task UpdateAsync(Author author, CancellationToken cancellationToken = default);
		Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
	}
}
