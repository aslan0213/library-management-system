using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Auth
{
	public class RefreshTokenRequest
	{
		public string RefreshToken { get; set; } = string.Empty;
	}
}
