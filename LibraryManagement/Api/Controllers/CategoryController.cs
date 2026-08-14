using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Applications;
using Shared.Dtos.Category;
using Shared.Paging;
namespace LibraryManagement.Api.Controllers
{
	/// <summary>
	/// Manages book categories.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryAppService _categoryAppService;

		public CategoryController(ICategoryAppService categoryAppService)
		{
			_categoryAppService = categoryAppService;
		}

		[HttpGet]
		[ProducesResponseType(typeof(PagedResult<CategoryResponse>), StatusCodes.Status200OK)]
		public async Task<ActionResult<PagedResult<CategoryResponse>>> GetPaged(
			[FromQuery] PagedRequest request,
			CancellationToken cancellationToken)
		{
			var result = await _categoryAppService.GetPagedAsync(request, cancellationToken);
			return Ok(result);
		}

		[HttpGet("{id:guid}")]
		[ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<CategoryResponse>> GetById(Guid id, CancellationToken cancellationToken)
		{
			var category = await _categoryAppService.GetByIdAsync(id, cancellationToken);
			if(category == null)
			{
				return NotFound();
			}
			return Ok(category);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<ActionResult<CategoryResponse>> Create(
			[FromBody] CreateCategoryRequest request,
			CancellationToken cancellationToken)
		{
			var created = await _categoryAppService.CreateAsync(request, cancellationToken);
			return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
		}

		[HttpPut("{id:guid}")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update(
			Guid id,
			[FromBody] UpdateCategoryRequest request,
			CancellationToken cancellationToken)
		{
			await _categoryAppService.UpdateAsync(id, request, cancellationToken);
			return NoContent();
		}

		[HttpDelete("{id:guid}")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
		{
			await _categoryAppService.DeleteAsync(id, cancellationToken);
			return NoContent();
		}
	}
}
