using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Shared.Dtos.Member;

namespace Services.Validation
{
	public class UpdateMemberRequestValidator : AbstractValidator<UpdateMemberRequest>
	{
		public UpdateMemberRequestValidator()
		{
			RuleFor(x => x.FirstName)
				.NotEmpty()
				.MaximumLength(100);
			RuleFor(x => x.LastName)
				.NotEmpty()
				.MaximumLength(100);
			RuleFor(x => x.Email)
				.NotEmpty()
				.MaximumLength(256)
				.EmailAddress();
			RuleFor(x => x.PhoneNumber)
				.MaximumLength(20);
		}
	}
}
