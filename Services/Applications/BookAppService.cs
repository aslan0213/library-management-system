using Abstractions.Services;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using Shared.Dtos.Book;
using Shared.Paging;
using Abstractions.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public class BookAppService : IBookAppService
	{
		private readonly IBookService _bookService;
		private readonly IMapper _mapper;
		private readonly IValidator<CreateBookRequest> _createValidator;
		private readonly IValidator<UpdateBookRequest> _updateValidator;

		public BookAppService(
			IBookService bookService,
			IMapper mapper,
			IValidator<CreateBookRequest> createValidator,
			IValidator<UpdateBookRequest> updateValidator)
		{
			_bookService = bookService;
			_mapper = mapper;
			_createValidator = createValidator;
			_updateValidator = updateValidator;
		}

		public async Task<BookResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var book = await _bookService.GetByIdAsync(id, cancellationToken);
			return book is null ? null : _mapper.Map<BookResponse>(book);
		}

		public async Task<Shared.Paging.PagedResult<BookResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
		{
			var query = _mapper.Map<PagedQuery>(request);
			var result = await _bookService.GetPagedAsync(query, cancellationToken);
			return _mapper.Map<Shared.Paging.PagedResult<BookResponse>>(result);
		}

		public async Task<BookResponse> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default)
		{
			await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

			var book = _mapper.Map<Book>(request);
			var created = await _bookService.CreateAsync(book, request.CategoryIds, cancellationToken);
			return _mapper.Map<BookResponse>(created);
		}

		public async Task UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken cancellationToken = default)
		{
			await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

			var book = _mapper.Map<Book>(request);
			book.Id = id;
			await _bookService.UpdateAsync(book, request.CategoryIds, cancellationToken);
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
			await _bookService.DeleteAsync(id, cancellationToken);
	}
}
