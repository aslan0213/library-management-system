using Shared.Dtos.Book;
using Shared.Dtos.Member;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Loan
{
	public class LoanResponse
	{
		public Guid Id { get; set; }
		public BookResponse Book { get; set; } = null!;
		public MemberResponse Member { get; set; } = null!;
		public DateTime BorrowedDate { get; set; }
		public DateTime DueAt { get; set; }
		public DateTime? ReturnedDate { get; set; }
	}
}
