using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Shared.Dtos.Category;

namespace Services.Validation
{
	public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
	{
		public CreateCategoryRequestValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty()
				.MaximumLength(100);
		}
	}
}
