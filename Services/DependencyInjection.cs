using Abstractions.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Applications;
using Services.Auth;
using Services.BackgroundJobs;
using Services.FileService;
using Services.Notifications;
using Services.Reservations;
using Shared.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
namespace Services
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
		{
			//mapping
			services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);
			//validation
			services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
			//services
			services.AddScoped<IAuthorService, AuthorService>();
			services.AddScoped<IBookService, BookService>();
			services.AddScoped<IMemberService, MemberService>();
			services.AddScoped<ILoanService, LoanService>();
			services.AddScoped<IReservationService, ReservationService>();
			services.AddScoped<IReservationExpirationService, ReservationExpirationService>();
			services.AddHostedService<ReservationExpirationBackgroundService>();
			services.AddHostedService<RefreshTokenCleanupBackgroundService>();
			services.AddScoped<INotificationService, NotificationService>();
			services.AddScoped<ICategoryService, CategoryService>();
			services.AddScoped<IPublisherService, PublisherService>();
			//appservices
			services.AddScoped<IAuthorAppService, AuthorAppService>();
			services.AddScoped<IMemberAppService, MemberAppService>();
			services.AddScoped<IBookAppService, BookAppService>();
			services.AddScoped<ILoanAppService, LoanAppService>();
			services.AddScoped<IReservationAppService, ReservationAppService>();
			services.AddScoped<INotificationAppService, NotificationAppService>();	
			services.AddScoped<ICategoryAppService, CategoryAppService>();
			services.AddScoped<IPublisherAppService, PublisherAppService>();
			//Authentication
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IJwtTokenGenerator, JwtGenerator>();
			services.AddScoped<IPasswordHasher, PasswordHasher>();
			services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
			// File storage
			services.Configure<FileStorageSettings>(configuration.GetSection(FileStorageSettings.SectionName));
			services.AddSingleton<IFileService,LocalFileService>();
			// Async notification dispatcher
			var notificationChannel = Channel.CreateUnbounded<Guid>(new UnboundedChannelOptions
			{
				SingleReader = true,
				SingleWriter = false
			});
			services.AddSingleton(notificationChannel);
			services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
			services.AddHostedService<NotificationDispatchBackgroundService>();
			return services;
		}	
	}
}
