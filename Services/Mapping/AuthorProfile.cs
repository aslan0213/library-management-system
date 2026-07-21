using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Shared.Dtos.Author;
using Domain.Entities;
namespace Services.Mapping
{
	public class AuthorProfile:Profile
	{
		public AuthorProfile()
		{
			CreateMap<Author, AuthorResponce>();
			CreateMap<CreateAuthorRequest, Author>();
			CreateMap<UpdateAuthorRequest, Author>();
		}
	}

}
