using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions
{
	public class EmailAlreadyRegisteredException : Exception
	{
		public EmailAlreadyRegisteredException(string email) : base($"An account with the email address '{email}' is already registered.")
		{
		}
	}
}
