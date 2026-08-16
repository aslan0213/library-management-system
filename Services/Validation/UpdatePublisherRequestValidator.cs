using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Shared.Dtos.Publisher;
namespace Services.Validation
{
	public class UpdatePublisherRequestValidator : AbstractValidator<UpdatePublisherRequest>
	{
		public UpdatePublisherRequestValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty()
				.MaximumLength(150);
			RuleFor(x => x.Country)
				.MaximumLength(100);
		}
	}
}
