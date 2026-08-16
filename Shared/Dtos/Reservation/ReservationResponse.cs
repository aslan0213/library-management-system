using Shared.Dtos.Book;
using Shared.Dtos.Member;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Reservation
{
	public class ReservationResponse
	{
		public Guid Id { get; set; }
		public BookResponse Book { get; set; } = null!;
		public MemberResponse Member { get; set; } = null!;
		public DateTime ReservedAt { get; set; }
		public string Status { get; set; } = string.Empty;
		public DateTime? HeldUntil { get; set; }

	}
}
