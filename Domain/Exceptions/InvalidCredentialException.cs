using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions
{
	public class InvalidCredentialException : Exception
	{
		public InvalidCredentialException():base("Invalid email or password.")
		{

		}
	}
}
