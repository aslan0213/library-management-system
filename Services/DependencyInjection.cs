using Abstractions.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Applications;
using Services.Auth;
using Services.BackgroundJobs;
using Services.Reservations;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;
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
			//appservices
			services.AddScoped<IAuthorAppService, AuthorAppService>();
			services.AddScoped<IMemberAppService, MemberAppService>();
			services.AddScoped<IBookAppService, BookAppService>();
			services.AddScoped<ILoanAppService, LoanAppService>();
			//Authentication
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IJwtTokenGenerator, JwtGenerator>();
			services.AddScoped<IPasswordHasher, PasswordHasher>();
			services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
			return services;
		}	
	}
}
