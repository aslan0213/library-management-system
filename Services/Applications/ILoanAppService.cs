using Shared.Dtos.Loan;
using Shared.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public interface ILoanAppService
	{
		Task<LoanResponce?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<PagedResult<LoanResponce>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
		Task<LoanResponce> CreateLoanAsync(CreateLoanRequest request, CancellationToken cancellationToken = default);
		Task ReturnLoanAsync(Guid loanId, CancellationToken cancellationToken = default);
	}
}
