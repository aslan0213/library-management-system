using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Reservation
{
	public class CreateReservationRequest
	{
		public Guid BookId { get; set; }
		public Guid MemberId { get; set; }
	}
}
