using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Shared.Dtos.Reservation;
namespace Services.Validation
{
	public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
	{
		public CreateReservationRequestValidator()
		{
			RuleFor(x => x.BookId)
				.NotEmpty();
			RuleFor(x => x.MemberId)
				.NotEmpty();
		}
	}
}
