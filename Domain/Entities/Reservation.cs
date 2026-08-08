using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public enum ReservationStatus
	{
		Pending,
		Fullfilled,
		Canceled
	}
	public class Reservation : IHasId
	{
		public Guid Id { get; set; }
		public Guid BookId { get; set; }
		public Book Book { get; set; } = null!;
		public Guid MemberId { get; set; }
		public Member Member { get; set; } = null!;
		public DateTime ReservedAt { get; set; }
		public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
	}
}
