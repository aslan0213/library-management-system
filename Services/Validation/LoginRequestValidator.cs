using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Shared.Dtos.Auth;
namespace Services.Validation
{
	public class LoginRequestValidator : AbstractValidator<LoginRequest>
	{
		public LoginRequestValidator()
		{
			RuleFor(x => x.Email).NotEmpty();
			RuleFor(x => x.Password).NotEmpty();
		}
	}
}
