using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Shared.Dtos.Book;
namespace Services.Validation
{
	public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
	{
		public UpdateBookRequestValidator()
		{
			RuleFor(x => x.Title)
			.NotEmpty()
			.MaximumLength(200);

			RuleFor(x => x.Isbn)
				.NotEmpty()
				.Length(10, 13);

			RuleFor(x => x.Publisher)
				.NotEmpty()
				.MaximumLength(150);

			RuleFor(x => x.PublishedYear)
				.InclusiveBetween(1450, DateTime.UtcNow.Year);

			RuleFor(x => x.TotalCopies)
				.GreaterThanOrEqualTo(0);

			RuleFor(x => x.AuthorId)
				.NotEmpty();
		}
	}
}
