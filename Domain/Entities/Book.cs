using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public class Book
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Isbn { get; set; } = string.Empty;
		public int PublishedYear { get; set; }
		public string Publisher { get; set; }
		public int TotalCopies { get; set; }
		public int AvailableCopies { get; set; }
		public Guid AuthorId { get; set; }
		public Author Author { get; set; } = null!;
		public ICollection<Loan> Loans { get; set; } = new List<Loan>();
	}
}
