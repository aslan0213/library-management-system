using Microsoft.AspNetCore.Mvc;
using Services.Applications;
using Shared.Dtos.Author;
using Shared.Paging;
namespace LibraryManagement.Api.Controllers
{
	/// <summary>
	///	 Manages library Authors
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class AuthorController: ControllerBase
	{
		private readonly IAuthorAppService _authorAppService;
		public AuthorController(IAuthorAppService authorAppService)
		{
			_authorAppService = authorAppService;
		}

		/// <summary>
		/// Gets a paged, sortable list of authors.
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<PagedResult<AuthorResponse>>> GetPaged([FromQuery] PagedRequest request, CancellationToken cancellationToken)
		{
			var authors = await _authorAppService.GetPagedAsync(request, cancellationToken);
			return Ok(authors);
		}
		/// <summary>
		/// Gets a single author by ID.
		/// </summary>	
		[HttpGet("{id:guid}")]
		public async Task<ActionResult<AuthorResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
		{
			var author = await _authorAppService.GetByIdAsync(id, cancellationToken);
			if (author == null)
			{
				return NotFound();
			}
			return Ok(author);
		}
		/// <summary>
		/// Creates a new author.
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<AuthorResponse>> Create([FromBody] CreateAuthorRequest request, CancellationToken cancellationToken)
		{
			var author = await _authorAppService.CreateAsync(request, cancellationToken);
			return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
		}
		/// <summary>
		/// Updates an existing author.
		/// </summary>
		[HttpPut("{id:guid}")]
		public async Task<ActionResult> Update(Guid id, [FromBody] UpdateAuthorRequest request, CancellationToken cancellationToken)
		{
			await _authorAppService.UpdateAsync(id, request, cancellationToken);
			return NoContent();
		}

		/// <summary>
		/// Deletes an author.
		/// </summary>
		[HttpDelete("{id:guid}")]
		public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
		{
			await _authorAppService.DeleteAsync(id, cancellationToken);
			return NoContent();
		}
	}
}
