using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Book
{
	public class CreateBookRequest
	{
		public string Title { get; set; } = string.Empty;
		public string Isbn { get; set; } = string.Empty;
		public int PublishedYear { get; set; }
		public string Publisher { get; set; } = string.Empty;
		public int TotalCopies { get; set; }
		public Guid AuthorId { get; set; }

	}
}
