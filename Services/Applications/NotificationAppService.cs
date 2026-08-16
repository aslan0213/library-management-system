using Abstractions.Services;
using AutoMapper;
using FluentValidation;
using Shared.Dtos.Notification;
using Shared.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public class NotificationAppService : INotificationAppService
	{
		private readonly INotificationService _notificationService;
		private readonly IMapper _mapper;
		public NotificationAppService(INotificationService notificationService, IMapper mapper)
		{
			_notificationService = notificationService;
			_mapper = mapper;
		}
		public async Task<NotificationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var notification = await _notificationService.GetByIdAsync(id, cancellationToken);
			return notification is null ? null : _mapper.Map<NotificationResponse>(notification);
		}

		public async Task<PagedResult<NotificationResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
		{
			var query = _mapper.Map<Abstractions.Paging.PagedQuery>(request);
			var notifications = await _notificationService.GetPagedAsync(query, cancellationToken);
			return _mapper.Map<PagedResult<NotificationResponse>>(notifications);
		}

		public async Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
		{
			await _notificationService.MarkAsReadAsync(id, cancellationToken);
		}
	}
}
