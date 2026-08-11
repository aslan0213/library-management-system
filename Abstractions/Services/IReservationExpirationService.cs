using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions.Services
{
	public interface IReservationExpirationService
	{
		Task ProcessExpiredReservationsAsync(CancellationToken cancellationToken = default);
	}
}
