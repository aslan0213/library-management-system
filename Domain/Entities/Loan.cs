using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public class Loan : IHasId
	{
		public Guid Id { get; set; }
		public Guid BookId { get; set; }
		public Book Book { get; set; } = null!;
		public Guid MemberId { get; set; }
		public Member Member { get; set; } = null!;
		public DateTime BorrowedAt { get; set; }
		public DateTime DueAt { get; set; }	
		public DateTime? ReturnedAt { get; set; }
	}
}
