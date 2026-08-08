using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions.Services
{
	public class AuthResult
	{
		public User User { get; set; } = null!;
		public string AccessToken { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
		public DateTime ExpiredAt { get; set; }
	}
}
