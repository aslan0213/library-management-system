using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Auth
{
	public class TokenResponse
	{
		public string AccessToken { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
		public DateTime ExpiresAt { get; set; }
	}
}
