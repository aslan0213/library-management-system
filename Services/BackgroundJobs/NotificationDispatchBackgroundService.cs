using Abstractions.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Services.BackgroundJobs
{
	public class NotificationDispatchBackgroundService : BackgroundService
	{
		private readonly Channel<Guid> _channel;
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<NotificationDispatchBackgroundService> _logger;
		public NotificationDispatchBackgroundService(
			Channel<Guid> channel,
			IServiceScopeFactory scopeFactory, 
			ILogger<NotificationDispatchBackgroundService> logger)
		{
			_channel = channel;
			_scopeFactory = scopeFactory;
			_logger = logger;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Notification dispatch background service started");
			await foreach (var notificationId in _channel.Reader.ReadAllAsync(stoppingToken))
			{
				try
				{
					using var scope = _scopeFactory.CreateScope();
					var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
					var notification = await unitOfWork.Notifications.GetByIdAsync(notificationId, stoppingToken);
					if (notification is null) 
					{
						_logger.LogWarning("Notification {NotificationId} not found, skipping dispatch", notificationId);
						continue;
					}
					// external dispatch 
					_logger.LogInformation("Dispatching notification {NotificationId} to member {MemberId}: {Message}", notification.Id, notification.MemberId, notification.Message);
					await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken); // network latency
					_logger.LogInformation("Notification {NotificationId} dispatched successfully", notification.Id);
				}
				catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
				{
					break;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error dispatching notification with ID {NotificationId}", notificationId);
				}
			}
			_logger.LogInformation("Notification dispatch background service stopped");
		}
	}
}
