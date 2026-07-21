using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Shared.Dtos.Member;
using Domain.Entities;
namespace Services.Mapping
{
	public class MemberProfile: Profile
	{
		public MemberProfile() 
		{ 
			CreateMap<Member, MemberResponce>();
			CreateMap<CreateMemberRequest, Member>();
			CreateMap<UpdateMemberRequest, Member>();
		}
	}
}
