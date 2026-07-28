using Microsoft.AspNetCore.Mvc;
using Services.Applications;
using Shared.Dtos.Book;
using Shared.Paging;
namespace LibraryManagement.Api.Controllers
{
	/// <summary>
	///	Manages library books.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class BookController : ControllerBase
	{
		private readonly IBookAppService _bookAppService;

		public BookController(IBookAppService bookAppService)
		{
			_bookAppService = bookAppService;
		}

		/// <summary>
		/// Gets a paged, sortable list of books.
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<PagedResult<BookResponse>>> GetPaged(
			[FromQuery] PagedRequest request,
			CancellationToken cancellationToken)
		{
			var result = await _bookAppService.GetPagedAsync(request, cancellationToken);
			return Ok(result);
		}

		/// <summary>
		/// Gets a single book by Id.
		/// </summary>
		[HttpGet("{id:guid}")]
		public async Task<ActionResult<BookResponse>> GetById(Guid id, CancellationToken cancellationToken)
		{
			var book = await _bookAppService.GetByIdAsync(id, cancellationToken);
			return book is null ? NotFound() : Ok(book);
		}

		/// <summary>
		/// Adds a new book to the catalog.
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<BookResponse>> Create(
			[FromBody] CreateBookRequest request,
			CancellationToken cancellationToken)
		{
			var created = await _bookAppService.CreateAsync(request, cancellationToken);
			return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
		}

		/// <summary>
		/// Updates an existing book.
		/// </summary>
		[HttpPut("{id:guid}")]
		public async Task<IActionResult> Update(
			Guid id,
			[FromBody] UpdateBookRequest request,
			CancellationToken cancellationToken)
		{
			await _bookAppService.UpdateAsync(id, request, cancellationToken);
			return NoContent();
		}

		/// <summary>
		/// Removes a book from the catalog.
		/// </summary>
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
		{
			await _bookAppService.DeleteAsync(id, cancellationToken);
			return NoContent();
		}
	}
}
