using Abstractions.Paging;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions.Services
{
	public interface IMemberService
	{
		Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<PagedResult<Member>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
		Task<Member> CreateAsync(Member member, CancellationToken cancellationToken = default);
		Task UpdateAsync(Member member, CancellationToken cancellationToken = default);
		Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
	}
}

