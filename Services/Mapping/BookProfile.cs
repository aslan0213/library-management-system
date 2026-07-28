using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Shared.Dtos.Book;
using Domain.Entities;
namespace Services.Mapping
{
	public class BookProfile: Profile
	{
		public BookProfile()
		{
			CreateMap<Book, BookResponse>();
			CreateMap<CreateBookRequest, Book>();
			CreateMap<UpdateBookRequest, Book>();
		}
	}
}
