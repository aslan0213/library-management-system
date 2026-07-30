using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;

namespace Abstractions.Repositories
{
	public interface IUserRepository : IRepositoryBase<User>
	{
		Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	}
}
