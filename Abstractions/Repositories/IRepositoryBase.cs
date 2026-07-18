using Abstractions.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions.Repositories
{
	public interface IRepositoryBase<T> where T : class
	{
		Task<T?> GetByIdAsync(Guid Id, CancellationToken cancellationToken=default);
		Task<PagedResult<T>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
		Task AddAsync(T entity, CancellationToken cancellationToken = default);
		void Update(T entity);
		void Delete(T entity);
	}
}
