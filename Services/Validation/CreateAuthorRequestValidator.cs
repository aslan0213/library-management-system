using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Shared.Dtos.Author;
namespace Services.Validation
{
	public class CreateAuthorRequestValidator : AbstractValidator<CreateAuthorRequest>
	{
		public CreateAuthorRequestValidator()
		{
			RuleFor(x => x.FirstName)
				.NotEmpty()
				.MaximumLength(100);
			RuleFor(x => x.LastName)
				.NotEmpty()
				.MaximumLength(100);
			RuleFor(x => x.Bio)
				.MaximumLength(1000);
			RuleFor(x => x.DateOfBirth)
				.LessThan(DateTime.UtcNow)
				.When(x=>x.DateOfBirth.HasValue);
		}
	}
}
