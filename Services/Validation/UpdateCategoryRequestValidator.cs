using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Shared.Dtos.Category;
namespace Services.Validation
{
	public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
	{
		public UpdateCategoryRequestValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty()
				.MaximumLength(100);
		}
	}
}
