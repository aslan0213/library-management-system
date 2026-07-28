using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Abstractions.Paging;
using Shared.Paging;
namespace Services.Mapping
{
	public class PagingProfile : Profile
	{
		public PagingProfile()
		{
			CreateMap<PagedRequest, PagedQuery>()
				.ForMember(dest => dest.SortDirection, opt => opt.MapFrom(src =>
					ParseSortDirection(src.SortDirection)));

			CreateMap(typeof(Abstractions.Paging.PagedResult<>), typeof(Shared.Paging.PagedResult<>));
		}
		private static SortDirection ParseSortDirection(string? value) =>
			string.Equals(value, "Descending", StringComparison.OrdinalIgnoreCase)
				? SortDirection.Descending
				: SortDirection.Ascending;

	}
}
