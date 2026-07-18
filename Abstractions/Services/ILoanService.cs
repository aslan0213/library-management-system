using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;

namespace Abstractions.Services
{
	public interface ILoanService
	{
		Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<PagedResult<Loan>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
		Task<Loan> CreateLoanAsync(Guid bookId, Guid memberId, DateTime dueAt, CancellationToken cancellationToken = default);
		Task ReturnLoanAsync(Guid loanId, CancellationToken cancellationToken = default);
	}
}

