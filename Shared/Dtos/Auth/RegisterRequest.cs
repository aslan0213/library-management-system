using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Auth
{
	public class RegisterRequest
	{
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}
}
