using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Book
{
	public class BookSearchRequest
	{
		public string? Title { get; set; }
		public Guid? AuthorId { get; set; }
		public Guid? PublisherId { get; set; }
		public Guid? CategoryId { get; set; }
		public int? MinYear { get; set; }
		public int? MaxYear { get; set; }
		public bool? OnlyAvailable { get; set; }
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
	}
}
