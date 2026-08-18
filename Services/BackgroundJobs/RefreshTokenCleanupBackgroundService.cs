using Abstractions.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.BackgroundJobs
{
	public class RefreshTokenCleanupBackgroundService : BackgroundService
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<RefreshTokenCleanupBackgroundService> _logger;
		private static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(1); // her deqiqe yoxlanacaq

		public RefreshTokenCleanupBackgroundService(
			IServiceScopeFactory scopeFactory, 
			ILogger<RefreshTokenCleanupBackgroundService> logger)
		{
			_scopeFactory = scopeFactory;
			_logger = logger;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while(!stoppingToken.IsCancellationRequested)
			{
				try
				{
					using var scope = _scopeFactory.CreateScope();
					var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
					var deletedCount = await unitOfWork.RefreshTokens.DeleteExpiredAsync(DateTime.UtcNow, stoppingToken);
					if (deletedCount > 0) 
					{ 
						_logger.LogInformation("Cleaned up {Count} expired/revoked refresh tokens.", deletedCount);
					}
				}
				catch (Exception ex) when (ex is not OperationCanceledException)
				{
					_logger.LogError(ex, "An error occurred while cleaning up expired refresh tokens.");
				}
				await Task.Delay(CleanupInterval, stoppingToken);
			}
		}
	}
}
