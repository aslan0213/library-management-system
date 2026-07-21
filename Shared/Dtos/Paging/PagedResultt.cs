using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Paging
{
	public class PagedResultt
	{
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string? SortBy { get; set; }
		public string? SortDirection { get; set; } = "Ascending";
	}
}
