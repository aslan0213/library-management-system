using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Shared.Dtos.Auth;
namespace Services.Validation
{
	public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
	{
		public RegisterRequestValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty()
				.MaximumLength(256)
				.EmailAddress();
			RuleFor(x => x.Password)
				.NotEmpty()
				.MinimumLength(8)
				.Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
				.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
				.Matches("[0-9]").WithMessage("Password must contain at least one digit.");
		}
	}
}
