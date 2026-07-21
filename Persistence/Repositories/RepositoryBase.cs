using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;
using Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Persistence.Repositories
{
	public class RepositoryBase<T> : IRepositoryBase<T> where T : class, IHasId
	{
		protected readonly LibraryDbContext _context;
		public RepositoryBase(LibraryDbContext context)
		{
			_context = context; 
		}
		protected IQueryable<T> FindAll(bool trackChanges) =>
			trackChanges ? _context.Set<T>() : _context.Set<T>().AsNoTracking();

		public virtual async Task<T?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default)
		{
			var query = IncludeRelated(FindAll(trackChanges: false));
			return await query.FirstOrDefaultAsync(x => x.Id == Id, cancellationToken);
		}

		public virtual async Task<PagedResult<T>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
		{
			var source = IncludeRelated(FindAll(trackChanges: false));
			source = ApplySort(source, query);
			var totalCount = await source.CountAsync(cancellationToken);
			var items = await source
				.Skip((query.PageNumber-1)*query.PageSize)
				.Take(query.PageSize)
				.ToListAsync(cancellationToken);
			return new PagedResult<T>
			{
				Items = items,
				TotalCount = totalCount,
				PageNumber = query.PageNumber,
				PageSize = query.PageSize
			};
		}

		public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
		{
		    await _context.Set<T>().AddAsync(entity, cancellationToken);
		}

		public void Update(T entity)
		{
			_context.Set<T>().Update(entity);
		}

		public void Delete(T entity)
		{
			_context.Set<T>().Remove(entity);
		}

		protected virtual IQueryable<T> IncludeRelated(IQueryable<T> source) => source;
		protected virtual IQueryable<T> ApplySort(IQueryable<T> source, PagedQuery query) => source;
	}
}
