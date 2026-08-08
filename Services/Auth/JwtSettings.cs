using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Auth
{
	public class JwtSettings
	{
		public string Secret { get; set; } = string.Empty;
		public string Issuer { get; set; } = string.Empty;	
		public string Audience { get; set; } = string.Empty;
		public int AccessTokenMinutes { get; set; } = 15;
		public int RefreshTokenDays { get; set; } = 7;
	}
}
