using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Shared.Dtos.Book;
using Shared.Dtos.Author;
namespace Services.Validation
{
	public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
	{
		public CreateBookRequestValidator()
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
				.InclusiveBetween(1450, DateTime.Now.Year);
			RuleFor(x => x.TotalCopies)
				.GreaterThanOrEqualTo(0);
			RuleFor(x => x.AuthorId)
				.NotEmpty();
			RuleForEach(x => x.CategoryIds)
				.NotEmpty();
		}
	}
}
