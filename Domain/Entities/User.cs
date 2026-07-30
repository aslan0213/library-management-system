using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public class User : IHasId
	{
		public Guid Id { get; set; }
		public string Email { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
		public Role role { get; set; } = Role.User;
		public DateTime CreatedAt { get; set; } 
		public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();


	}
}
