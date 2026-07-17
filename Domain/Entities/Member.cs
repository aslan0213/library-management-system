using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public class Member
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string? PhoneNumber { get; set; }
		public DateTime MembershipDate { get; set; }
		public ICollection<Loan> Loans { get; set; } = new List<Loan>();
	}
}
