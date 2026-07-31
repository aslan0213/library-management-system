using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Persistence.Repositories
{
	public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
	{
		public RefreshTokenRepository(LibraryDbContext context): base(context)
		{
			
		}

		public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)=>
			await FindAll(trackChanges: true)
				.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);
	}
}
