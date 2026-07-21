using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Loan
{
	public class CreateLoanRequest
	{
		public Guid BookId { get; set; }
		public Guid MemberId { get; set; }
		public DateTime DueDate { get; set; }

	}
}
