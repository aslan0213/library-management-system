using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Member
{
	public class MemberResponse
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string? PhoneNumber { get; set; }
		public DateTime MembershipDate { get; set; }

	}
}
