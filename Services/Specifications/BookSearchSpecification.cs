using Abstractions.Specifications;
using System;
using Domain.Entities;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications
{
	public class BookSearchSpecification : SpecificationBase<Book>
	{
		public BookSearchSpecification(
			string? Title,
			Guid? AuthorId,
			Guid? PublisherId, 
			Guid? CategoryId, 
			int? MinYear,
			int? MaxYear, 
			bool? OnlyAvailable)
		{
			Criteria = b =>
				(string.IsNullOrWhiteSpace(Title) || b.Title.Contains(Title)) &&
				(!AuthorId.HasValue || b.AuthorId == AuthorId.Value) &&
				(!PublisherId.HasValue || b.PublisherId == PublisherId.Value) &&
				(!CategoryId.HasValue || b.Categories.Any(c => c.Id == CategoryId.Value)) &&
				(!MinYear.HasValue || b.PublishedYear >= MinYear.Value) &&
				(!MaxYear.HasValue || b.PublishedYear <= MaxYear.Value) &&
				(!OnlyAvailable.HasValue || !OnlyAvailable.Value || b.AvailableCopies > 0);

			AddInclude(b => b.Author);
			AddInclude(b => b.Publisher);
			AddInclude(b => b.Categories);
			AddOrderBy(b => b.Title);

		}
	}
}
