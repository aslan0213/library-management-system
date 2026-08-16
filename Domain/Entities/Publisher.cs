using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public class Publisher : IHasId
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Country { get; set; }
		public ICollection<Book> Books { get; set; } = new List<Book>();
	}
}
