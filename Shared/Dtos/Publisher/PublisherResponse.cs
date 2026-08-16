using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Publisher
{
	public class PublisherResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Country { get; set; }
	}
}
