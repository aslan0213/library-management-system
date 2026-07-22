using Shared.Dtos.Member;
using Shared.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public interface IMemberAppService
	{
		Task<MemberResponce?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<PagedResult<MemberResponce>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
		Task<MemberResponce> CreateAsync(CreateMemberRequest request, CancellationToken cancellationToken = default);
		Task UpdateAsync(Guid id, UpdateMemberRequest request, CancellationToken cancellationToken = default);
		Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
	}
}
