using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Notification
{
	public class NotificationResponse
	{
		public Guid Id { get; set; }
		public Guid MemberId { get; set; }
		public string Message { get; set; } = string.Empty;
		public DateTime SentAt { get; set; }
		public bool IsRead { get; set; }
	}
}
