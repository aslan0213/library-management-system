using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions
{
	public class InvalidRefreshTokenException : Exception
	{
		public InvalidRefreshTokenException() : base("Refresh token is invalid or has expired.")
		{
		}
	}
}
