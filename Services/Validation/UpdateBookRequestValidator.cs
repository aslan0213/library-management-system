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

			RuleFor(x => x.PublisherId)
				.NotEmpty();

			RuleFor(x => x.PublishedYear)
				.InclusiveBetween(1450, DateTime.UtcNow.Year);

			RuleFor(x => x.TotalCopies)
				.GreaterThanOrEqualTo(0);

			RuleFor(x => x.AuthorId)
				.NotEmpty();

			RuleForEach(x => x.CategoryIds)
				.NotEmpty();
		}
	}
}
