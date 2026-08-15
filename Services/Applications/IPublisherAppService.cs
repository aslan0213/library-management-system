using Shared.Dtos.Publisher;
using Shared.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public interface IPublisherAppService
	{
		Task<PublisherResponse?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);
		Task<PagedResult<PublisherResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
		Task<PublisherResponse> CreateAsync(CreatePublisherRequest request, CancellationToken cancellationToken = default);
		Task UpdateAsync(Guid Id, UpdatePublisherRequest request, CancellationToken cancellationToken = default);
		Task DeleteAsync(Guid Id, CancellationToken cancellationToken = default);
	}
}
