using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions.Services
{
	public interface IJwtTokenGenerator
	{
		(string token, DateTime ExpiredAt) GenerateAccessToken(User user);	
		string GenerateRefreshToken();
	}
}
