using Abstractions.Paging;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions.Services
{
	public interface IPublisherService
	{
		Task<Publisher?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<PagedResult<Publisher>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
		Task<Publisher> CreateAsync(Publisher publisher, CancellationToken cancellationToken = default);
		Task UpdateAsync(Publisher publisher, CancellationToken cancellationToken = default);
		Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
	}
}
