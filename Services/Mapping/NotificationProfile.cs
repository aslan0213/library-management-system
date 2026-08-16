using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Shared.Dtos.Notification;
using Domain.Entities;

namespace Services.Mapping
{
	public class NotificationProfile : Profile
	{
		public NotificationProfile()
		{
			CreateMap<Notification, NotificationResponse>();
		}
	}
}
