using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public class RefreshToken : IHasId
	{
		public Guid Id { get; set; }
		public string TokenHash { get; set; } = string.Empty;
		public DateTime ExpiresAt { get; set; }
		public DateTime CreatedAt { get; set; }
		public Guid UserId { get; set; }
		public User User { get; set; } = null!;
		public DateTime? RevokedAt { get; set; }
		public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;
	}
}
