using Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Persistence.Repositories
{
	public class UserRepository : RepositoryBase<User>, IUserRepository
	{
		public UserRepository(LibraryDbContext context):base(context)
		{
			
		}
		public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
			await FindAll(trackChanges: false)
			.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
	}
}
