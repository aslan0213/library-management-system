using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public class Book : IHasId
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Isbn { get; set; } = string.Empty;
		public int PublishedYear { get; set; }
		public int TotalCopies { get; set; }
		public int AvailableCopies { get; set; }
		public string? CoverImagePath { get; set; }

		public Guid AuthorId { get; set; }
		public Author Author { get; set; } = null!;

		public Guid PublisherId { get; set; }
		public Publisher Publisher { get; set; } = null!;	

		public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
		public ICollection<Category> Categories { get; set; } = new List<Category>();
		public ICollection<Loan> Loans { get; set; } = new List<Loan>();
	}
}
