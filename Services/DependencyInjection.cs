using System;
using System.Collections.Generic;
using System.Text;
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
			return services;
		}	
	}
}
