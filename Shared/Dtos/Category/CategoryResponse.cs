using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Category
{
	public class CategoryResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
	}
}
