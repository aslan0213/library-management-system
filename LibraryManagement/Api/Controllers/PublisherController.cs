using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Applications;
using Shared.Dtos.Publisher;
using Shared.Paging;
namespace LibraryManagement.Api.Controllers
{
	/// <summary>
	/// Manages book publishers.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class PublisherController : ControllerBase
	{
		private readonly IPublisherAppService _publisherAppService;
		public PublisherController(IPublisherAppService publisherAppService)
		{
			_publisherAppService = publisherAppService;
		}

		/// <summary>
		/// Gets a paged, sortable list of publishers.
		/// </summary>
		[HttpGet]
		[ProducesResponseType(typeof(PagedResult<PublisherResponse>), StatusCodes.Status200OK)]
		public async Task<ActionResult<PagedResult<PublisherResponse>>> GetPaged([FromQuery] PagedRequest request, CancellationToken cancellationToken = default)
		{
			var result = await _publisherAppService.GetPagedAsync(request, cancellationToken);
			return Ok(result);
		}

		/// <summary>
		/// Gets a single publisher by Id.
		/// </summary>
		[HttpGet("{id:guid}")]
		[ProducesResponseType(typeof(PublisherResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<PublisherResponse>> GetById(Guid id, CancellationToken cancellationToken = default)
		{
			var result = await _publisherAppService.GetByIdAsync(id, cancellationToken);
			if (result == null)
			{
				return NotFound();
			}
			return Ok(result);
		}

		/// <summary>
		/// Creates a new publisher.
		/// </summary>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(typeof(PublisherResponse), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<ActionResult<PublisherResponse>> Create([FromBody] CreatePublisherRequest request, CancellationToken cancellationToken = default)
		{
			var result = await _publisherAppService.CreateAsync(request, cancellationToken);
			return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
		}

		/// <summary>
		/// Updates an existing publisher.
		/// </summary>
		[HttpPut("{id:guid}")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePublisherRequest request, CancellationToken cancellationToken = default)
		{
			await _publisherAppService.UpdateAsync(id, request, cancellationToken);
			return NoContent();
		}

		/// <summary>
		/// Deletes a publisher.
		/// </summary>
		[HttpDelete("{id:guid}")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
		{
			await _publisherAppService.DeleteAsync(id, cancellationToken);
			return NoContent();
		}

	}
}
