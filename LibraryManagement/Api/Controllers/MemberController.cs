using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Applications;
using Shared.Dtos.Member;
using Shared.Paging;

namespace LibraryManagement.Api.Controllers
{
	/// <summary>
	/// Manages library members.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class MemberController : ControllerBase
	{
		private readonly IMemberAppService _memberAppService;

		public MemberController(IMemberAppService memberAppService)
		{
			_memberAppService = memberAppService;
		}

		/// <summary>
		/// Gets a paged, sortable list of members.
		/// </summary>
		[HttpGet]
		[Authorize]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(PagedResult<MemberResponse>), StatusCodes.Status200OK)]
		public async Task<ActionResult<PagedResult<MemberResponse>>> GetPaged(
			[FromQuery] PagedRequest request,
			CancellationToken cancellationToken)
		{
			var result = await _memberAppService.GetPagedAsync(request, cancellationToken);
			return Ok(result);
		}

		/// <summary>
		/// Gets a single member by Id.
		/// </summary>
		[HttpGet("{id:guid}")]
		[Authorize]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(MemberResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<MemberResponse>> GetById(Guid id, CancellationToken cancellationToken)
		{
			var member = await _memberAppService.GetByIdAsync(id, cancellationToken);
			return member is null ? NotFound() : Ok(member);
		}

		/// <summary>
		/// Registers a new member.
		/// </summary>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(typeof(MemberResponse), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<MemberResponse>> Create(
			[FromBody] CreateMemberRequest request,
			CancellationToken cancellationToken)
		{
			var created = await _memberAppService.CreateAsync(request, cancellationToken);
			return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
		}

		/// <summary>
		/// Updates an existing member.
		/// </summary>
		[HttpPut("{id:guid}")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update(
			Guid id,
			[FromBody] UpdateMemberRequest request,
			CancellationToken cancellationToken)
		{
			await _memberAppService.UpdateAsync(id, request, cancellationToken);
			return NoContent();
		}

		/// <summary>
		/// Deletes a member.
		/// </summary>
		[HttpDelete("{id:guid}")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]	
		public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
		{
			await _memberAppService.DeleteAsync(id, cancellationToken);
			return NoContent();
		}
	}
}
