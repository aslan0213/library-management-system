using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions
{
	public class NotFoundException : Exception
	{
		public NotFoundException(string message):base(message)
		{ 
		}
		public static NotFoundException ForEntity(string entityName, Guid id) =>
			new($"{entityName} with ID {id} was not found.");
	}
}
