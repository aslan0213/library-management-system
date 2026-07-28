using Abstractions.Services;
using AutoMapper;
using FluentValidation;
using Shared.Dtos.Loan;
using Shared.Paging;
using Abstractions.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public class LoanAppService : ILoanAppService
	{
		private readonly ILoanService _loanService;
		private readonly IMapper _mapper;
		private readonly IValidator<CreateLoanRequest> _createValidator;

		public LoanAppService(
			ILoanService loanService,
			IMapper mapper,
			IValidator<CreateLoanRequest> createValidator)
		{
			_loanService = loanService;
			_mapper = mapper;
			_createValidator = createValidator;
		}

		public async Task<LoanResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var loan = await _loanService.GetByIdAsync(id, cancellationToken);
			return loan is null ? null : _mapper.Map<LoanResponse>(loan);
		}

		public async Task<Shared.Paging.PagedResult<LoanResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
		{
			var query = _mapper.Map<PagedQuery>(request);
			var result = await _loanService.GetPagedAsync(query, cancellationToken);
			return _mapper.Map<Shared.Paging.PagedResult<LoanResponse>>(result);
		}

		public async Task<LoanResponse> CreateLoanAsync(CreateLoanRequest request, CancellationToken cancellationToken = default)
		{
			await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

			var loan = await _loanService.CreateLoanAsync(request.BookId, request.MemberId, request.DueAt, cancellationToken);
			return _mapper.Map<LoanResponse>(loan);
		}

		public async Task ReturnLoanAsync(Guid loanId, CancellationToken cancellationToken = default) =>
			await _loanService.ReturnLoanAsync(loanId, cancellationToken);
	}
}
