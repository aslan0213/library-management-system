using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
namespace Services
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);
			return services;
		}	
	}
}
