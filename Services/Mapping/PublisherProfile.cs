using AutoMapper;
using Domain.Entities;
using Shared.Dtos.Publisher;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Mapping
{
	public class PublisherProfile : Profile
	{
		public PublisherProfile()
		{
			CreateMap<Publisher, PublisherResponse>();
			CreateMap<CreatePublisherRequest, Publisher>();
			CreateMap<UpdatePublisherRequest, Publisher>();
		}
	}
}
