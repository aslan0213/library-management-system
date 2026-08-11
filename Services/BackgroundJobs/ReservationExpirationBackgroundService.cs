using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Services.BackgroundJobs
{
	public class ReservationExpirationBackgroundService : BackgroundService
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<ReservationExpirationBackgroundService> _logger;
		private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1); // her deqiqe yoxlanacaq

		public ReservationExpirationBackgroundService(IServiceScopeFactory scopeFactory, ILogger<ReservationExpirationBackgroundService> logger)
		{
			_scopeFactory = scopeFactory;
			_logger = logger;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					using var scope = _scopeFactory.CreateScope();
					var expirationService = scope.ServiceProvider.GetRequiredService<IReservationExpirationService>();
					await expirationService.ProcessExpiredReservationsAsync();
				}
				catch (Exception ex) when (ex is not OperationCanceledException)
				{
					_logger.LogError(ex, "An error occurred while expiring reservations.");
				}
				await Task.Delay(Interval, stoppingToken);
			}
		}
	}
}
