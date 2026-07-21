using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Author
{
	public class AuthorResponce
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string? Bio { get; set; }
		public DateTime? DateOfBirth { get; set; }
	}
}
