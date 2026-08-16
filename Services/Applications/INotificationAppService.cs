using Shared.Paging;
using Shared.Dtos.Notification;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public interface INotificationAppService
	{
		Task<NotificationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<PagedResult<NotificationResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
		Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);
	}
}
