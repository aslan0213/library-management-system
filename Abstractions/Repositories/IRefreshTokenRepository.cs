using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
namespace Abstractions.Repositories
{
	public interface IRefreshTokenRepository : IRepositoryBase<RefreshToken>
	{
		Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
	}
}
