
using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Shared.Dtos.Loan;
namespace Services.Validation
{
	public class CreateLoanRequestValidator : AbstractValidator<CreateLoanRequest>
	{
		public CreateLoanRequestValidator()
		{
			RuleFor(x => x.BookId).
				NotEmpty();
			RuleFor(x => x.MemberId).
				NotEmpty();
			RuleFor(x => x.DueAt).
				GreaterThan(DateTime.UtcNow);
		}
	}
}
