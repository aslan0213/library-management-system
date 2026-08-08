using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public class Notification : IHasId
	{
		public Guid Id { get; set; }
		public string Message { get; set; } = string.Empty;
		public Guid MemberId { get; set; }
		public Member Member { get; set; } = null!;
		public DateTime SentAt { get; set; }
		public bool IsRead { get; set; }
	}
}
