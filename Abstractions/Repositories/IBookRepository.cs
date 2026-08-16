using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;
using Domain.Entities;
using Abstractions.Specifications;
namespace Abstractions.Repositories
{
	public interface IBookRepository:IRepositoryBase<Book>
	{
		Task<PagedResult<Book>> SearchAsync(ISpecification<Book> specification, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
	}
}
