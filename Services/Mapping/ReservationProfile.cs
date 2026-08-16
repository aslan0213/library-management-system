using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Shared.Dtos.Reservation;
namespace Services.Mapping
{
	public class ReservationProfile : Profile
	{
		public ReservationProfile()
		{
			CreateMap<Reservation, ReservationResponse>()
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
		}
	}
}
