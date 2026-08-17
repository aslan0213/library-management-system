using Abstractions.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Applications;
using Services.Auth;
using Services.BackgroundJobs;
using Services.FileService;
using Services.Reservations;
using Shared.Settings;
using System;
using System.Collections.Generic;
using System.Text;
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
			var fileStorageSettings = new FileStorageSettings();
			configuration.GetSection(FileStorageSettings.SectionName).Bind(fileStorageSettings);
			services.Configure<FileStorageSettings>(configuration.GetSection(FileStorageSettings.SectionName));
			services.AddSingleton<IFileService>(new LocalFileService(fileStorageSettings.BasePath));
			return services;
		}	
	}
}
