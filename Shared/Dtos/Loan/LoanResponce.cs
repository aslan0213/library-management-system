using Shared.Dtos.Book;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Loan
{
	public class LoanResponce
	{
		public Guid Id { get; set; }
		public BookResponce Book { get; set; } = null!;
		public MemberResponce Member { get; set; } = null!;
		public DateTime BorrowedDate { get; set; }
		public DateTime DueDate { get; set; }
		public DateTime? ReturnedDate { get; set; }
	}
}
