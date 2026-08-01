using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Applications;
using Shared.Dtos.Loan;
using Shared.Paging;

namespace LibraryManagement.Api.Controllers
{
	/// <summary>
	/// Manages book loans — creation and returns.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class LoanController: ControllerBase
	{
		private readonly ILoanAppService _loanAppService;

		public LoanController(ILoanAppService loanAppService)
		{
			_loanAppService = loanAppService;
		}

		/// <summary>
		/// Gets a paged, sortable list of loans.
		/// </summary>
		[HttpGet]
		[Authorize]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(PagedResult<LoanResponse>), StatusCodes.Status200OK)]
		public async Task<ActionResult<PagedResult<LoanResponse>>> GetPaged(
			[FromQuery] PagedRequest request,
			CancellationToken cancellationToken)
		{
			var result = await _loanAppService.GetPagedAsync(request, cancellationToken);
			return Ok(result);
		}

		/// <summary>
		/// Gets a single loan by Id.
		/// </summary>
		[HttpGet("{id:guid}")]
		[Authorize]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(LoanResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<LoanResponse>> GetById(Guid id, CancellationToken cancellationToken)
		{
			var loan = await _loanAppService.GetByIdAsync(id, cancellationToken);
			return loan is null ? NotFound() : Ok(loan);
		}

		/// <summary>
		/// Creates a new loan, borrowing a book on behalf of a member.
		/// </summary>
		[HttpPost]
		[Authorize]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(LoanResponse), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<ActionResult<LoanResponse>> Create(
			[FromBody] CreateLoanRequest request,
			CancellationToken cancellationToken)
		{
			var created = await _loanAppService.CreateLoanAsync(request, cancellationToken);
			return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
		}

		/// <summary>
		/// Marks a loan as returned.
		/// </summary>
		[HttpPost("{id:guid}/return")]
		[Authorize]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(typeof(LoanResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<ActionResult<LoanResponse>> Return(Guid id, CancellationToken cancellationToken)
		{
			await _loanAppService.ReturnLoanAsync(id, cancellationToken);
			var updated = await _loanAppService.GetByIdAsync(id, cancellationToken);
			return Ok(updated);
		}
	}
}
