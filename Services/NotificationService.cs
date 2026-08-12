using Abstractions.Paging;
using Abstractions.Repositories;
using Abstractions.Services;
using Domain.Entities;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
namespace Services
{
	public class NotificationService : INotificationService
	{
		private readonly IUnitOfWork _unitOfWork;

		public NotificationService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
			await _unitOfWork.Notifications.GetByIdAsync(id, cancellationToken);

		public async Task<PagedResult<Notification>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default) =>
			await _unitOfWork.Notifications.GetPagedAsync(query, cancellationToken);

		public async Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var existing = await _unitOfWork.Notifications.GetByIdAsync(id, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Notification), id);
			existing.IsRead = true;
			_unitOfWork.Notifications.Update(existing);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
	}
}
