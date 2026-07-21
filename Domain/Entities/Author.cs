using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public class Author : IHasId
	{
		public Guid Id { get; set; }	
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string? Bio { get; set; }
		public DateTime? DateOfBirth { get; set; }


		public ICollection<Book> Books { get; set; } = new List<Book>();
	}
}
