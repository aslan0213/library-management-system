using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
namespace Abstractions.Repositories
{
	public interface ICategoryRepository : IRepositoryBase<Category>
	{
		Task<List<Category>> GetByIdsTrackedAsync(List<Guid> ids, CancellationToken cancellationToken = default);
	}
}
