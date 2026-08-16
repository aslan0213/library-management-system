using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Applications;
using Shared.Dtos.Reservation;
using Shared.Paging;
namespace LibraryManagement.Api.Controllers
{
	/// <summary>
	/// Manages book reservations and holds.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class ReservationController : ControllerBase
	{
		private readonly IReservationAppService _reservationAppService;
		public ReservationController(IReservationAppService reservationAppService)
		{
			_reservationAppService = reservationAppService;
		}

		/// <summary>
		/// Gets a paged, sortable list of reservations.
		/// </summary>
		[HttpGet]
		[ProducesResponseType(typeof(PagedResult<ReservationResponse>), StatusCodes.Status200OK)]
		public async Task<ActionResult<PagedResult<ReservationResponse>>> GetPaged([FromQuery] PagedRequest request, CancellationToken cancellationToken)
		{
			var result = await _reservationAppService.GetPagedAsync(request, cancellationToken);
			return Ok(result);
		}

		/// <summary>
		/// Gets a single reservation by Id.
		/// </summary>
		[HttpGet("{id:guid}")]
		[ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<ReservationResponse>> GetById(Guid id, CancellationToken cancellationToken)
		{
			var reservation = await _reservationAppService.GetByIdAsync(id, cancellationToken);
			if(reservation is null)
			{
				return NotFound();
			}
			return Ok(reservation);
		}

		/// <summary>
		/// Creates a new reservation for a currently unavailable book.
		/// </summary>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<IActionResult> Create([FromBody] CreateReservationRequest request, CancellationToken cancellationToken = default)
		{
			var result = await _reservationAppService.CreateReservationAsync(request, cancellationToken);
			return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
		}

		/// <summary>
		/// Cancels a pending or fulfilled reservation.
		/// </summary>
		[HttpPost("{id:guid}/cancel")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
		{
			await _reservationAppService.CancelReservationAsync(id, cancellationToken);
			return NoContent();
		}
	}
}
