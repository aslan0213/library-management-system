using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
namespace Services
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);
			services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
			services.AddScoped<IAuthorService, AuthorService>();
			services.AddScoped<IBookService, BookService>();
			services.AddScoped<IMemberService, MemberService>();
			services.AddScoped<ILoanService, LoanService>();

			return services;
		}	
	}
}
