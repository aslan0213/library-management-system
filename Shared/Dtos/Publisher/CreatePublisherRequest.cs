using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Publisher
{
	public class CreatePublisherRequest
	{
		public string Name { get; set; } = string.Empty;	
		public string? Country { get; set; }
	}
}
