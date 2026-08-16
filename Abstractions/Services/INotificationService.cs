using Abstractions.Paging;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions.Services
{
	public interface INotificationService
	{
		Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<PagedResult<Notification>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
		Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);
	}
}
