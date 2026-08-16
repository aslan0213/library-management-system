using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Domain.Entities;
using Shared.Dtos.Category;
namespace Services.Mapping
{
	public class CategoryProfile : Profile
	{
		public CategoryProfile()
		{
			CreateMap<Category, CategoryResponse>();
			CreateMap<CreateCategoryRequest, Category>();
			CreateMap<UpdateCategoryRequest, Category>();
		}
	}
}
