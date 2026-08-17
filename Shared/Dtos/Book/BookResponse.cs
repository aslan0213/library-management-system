using Shared.Dtos.Author;
using Shared.Dtos.Category;
using Shared.Dtos.Publisher;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Book
{
	public class BookResponse
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Isbn { get; set; } = string.Empty;
		public int PublishedYear { get; set; }
		public int TotalCopies { get; set; }
		public int AvailableCopies { get; set; }
		public string? CoverImagePath { get; set; }
		public AuthorResponse Author { get; set; } = null!;
		public PublisherResponse Publisher { get; set; } = null!;
		public List<CategoryResponse> Categories { get; set; } = new();


	}
}
