using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions.Paging
{
	public class PagedQuery
	{
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string? SortBy { get; set; }
		public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

	}
}
