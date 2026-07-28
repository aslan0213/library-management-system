using Shared.Dtos.Loan;
using Shared.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public interface ILoanAppService
	{
		Task<LoanResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<PagedResult<LoanResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
		Task<LoanResponse> CreateLoanAsync(CreateLoanRequest request, CancellationToken cancellationToken = default);
		Task ReturnLoanAsync(Guid loanId, CancellationToken cancellationToken = default);
	}
}
