using Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Services.Applications;
using Shared.Dtos.Book;
using Shared.Paging;
using Shared.Settings;
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
		private readonly IBookService _bookService;
		private readonly IFileService _fileService;
		private readonly FileStorageSettings _fileStorageSettings;

		//Magic bytes for file type verification
		private static readonly byte[] JpegMagicBytes = [0xFF, 0xD8, 0xFF];
		private static readonly byte[] PngMagicBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

		public BookController(
			IBookAppService bookAppService,
			IBookService bookService,
			IFileService fileService,
			IOptions<FileStorageSettings> fileStorageSettings)
		{
			_bookAppService = bookAppService;
			_bookService = bookService;
			_fileService = fileService;
			_fileStorageSettings = fileStorageSettings.Value;
		}

		/// <summary>
		/// Gets a paged, sortable list of books.
		/// </summary>
		[HttpGet]
		[Authorize]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(PagedResult<BookResponse>), StatusCodes.Status200OK)]
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
		[Authorize]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<BookResponse>> GetById(Guid id, CancellationToken cancellationToken)
		{
			var book = await _bookAppService.GetByIdAsync(id, cancellationToken);
			return book is null ? NotFound() : Ok(book);
		}

		/// <summary>
		/// Adds a new book to the catalog.
		/// </summary>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(typeof(BookResponse), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
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
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
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
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
		{
			await _bookAppService.DeleteAsync(id, cancellationToken);
			return NoContent();
		}

		/// <summary>
		/// Searches books with dynamic filters (title, author, publisher, category, year range, availability).
		/// </summary>
		[HttpGet("search")]
		[ProducesResponseType(typeof(PagedResult<BookResponse>), StatusCodes.Status200OK)]
		public async Task<ActionResult<PagedResult<BookResponse>>> Search([FromQuery] BookSearchRequest request, CancellationToken cancellationToken)
		{
			var result = await _bookAppService.SearchAsync(request, cancellationToken);
			return Ok(result);
		}

		/// <summary>
		/// Uploads a cover image for a book. Requires Admin role.
		/// Accepts JPEG and PNG files up to 5 MB. Validates file content via magic-byte verification.
		/// </summary>
		/// <param name="id">The book's unique identifier.</param>
		/// <param name="file">The image file (JPEG or PNG, max 5 MB).</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		[HttpPost("{id:guid}/cover")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
		[ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
		public async Task<IActionResult> UploadCover(Guid id, IFormFile file, CancellationToken cancellationToken)
		{
			var book = await _bookService.GetByIdAsync(id, cancellationToken);
			if (book is null)
				return NotFound();

			if (file.Length == 0)
				return BadRequest("File is empty.");
			if (file.Length > _fileStorageSettings.MaxFileSizeBytes)
				return StatusCode(StatusCodes.Status413PayloadTooLarge,
					$"File size exceeds the maximum allowed size of {_fileStorageSettings.MaxFileSizeBytes / (1024 * 1024)} MB.");

			var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
			if (!_fileStorageSettings.AllowedExtensions.Contains(extension))
				return StatusCode(StatusCodes.Status415UnsupportedMediaType,
					$"File type '{extension}' is not supported. Allowed types: {string.Join(", ", _fileStorageSettings.AllowedExtensions)}");

			await using (var validationStream = file.OpenReadStream())
			{
				if (!await IsValidImageAsync(validationStream, extension))
					return StatusCode(StatusCodes.Status415UnsupportedMediaType,
						"File content does not match the expected image format. The file may be corrupted or have an incorrect extension.");
			}
			
			if (!string.IsNullOrEmpty(book.CoverImagePath))
				await _fileService.DeleteFileAsync(book.CoverImagePath, cancellationToken);
			
			var fileName = $"{id}{extension}";
			string savedPath;
			await using (var saveStream = file.OpenReadStream())
			{
				savedPath = await _fileService.SaveFileAsync(saveStream, fileName, "covers", cancellationToken);
			}

			await _bookService.UpdateCoverPathAsync(id, savedPath, cancellationToken);
			return NoContent();
		}

		/// <summary>
		/// Downloads the cover image of a book. Returns the image file with the correct Content-Type.
		/// </summary>
		/// <param name="id">The book's unique identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		[HttpGet("{id:guid}/cover")]
		[Authorize]
		[ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DownloadCover(Guid id, CancellationToken cancellationToken)
		{
			var book = await _bookService.GetByIdAsync(id, cancellationToken);
			if (book is null)
				return NotFound();

			if (string.IsNullOrEmpty(book.CoverImagePath))
				return NotFound("This book does not have a cover image.");

			var result = await _fileService.GetFileAsync(book.CoverImagePath, cancellationToken);
			if (result is null)
				return NotFound("Cover image file not found on disk.");
			var (fileStream, contentType) = result.Value;
			return File(fileStream, contentType);
		}

		private static async Task<bool> IsValidImageAsync(Stream stream, string extension)
		{
			var headerBytes = new byte[8];
			var bytesRead = await stream.ReadAsync(headerBytes.AsMemory(0, 8));
			if (bytesRead < 3)
				return false;
			return extension switch
			{
				".jpg" or ".jpeg" => headerBytes[..3].SequenceEqual(JpegMagicBytes),
				".png" => bytesRead >= 8 && headerBytes.SequenceEqual(PngMagicBytes),
				_ => false
			};
		}
	}
}
