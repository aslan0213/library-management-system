using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public class Category : IHasId
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public ICollection<Book> Books { get; set; } = new List<Book>();
	}
}
