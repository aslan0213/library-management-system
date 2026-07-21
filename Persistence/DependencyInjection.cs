using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("DefaultConnection") 
				?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			services.AddDbContext<LibraryDbContext>(options =>
				options.UseNpgsql(connectionString));
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			return services;
		}
	}
}
