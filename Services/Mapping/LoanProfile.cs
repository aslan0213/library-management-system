using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Shared.Dtos.Loan;
using Domain.Entities;
namespace Services.Mapping
{
	public class LoanProfile : Profile
	{
		public LoanProfile()
		{
			CreateMap<Loan, LoanResponse>();
		}
	}
}
