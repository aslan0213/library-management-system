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
		private readonly ICacheService _cacheService;
		private readonly IMapper _mapper;
		private readonly IValidator<CreateBookRequest> _createValidator;
		private readonly IValidator<UpdateBookRequest> _updateValidator;

		private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
		public BookAppService(
			IBookService bookService,
			ICacheService cacheService,
			IMapper mapper,
			IValidator<CreateBookRequest> createValidator,
			IValidator<UpdateBookRequest> updateValidator)
		{
			_bookService = bookService;
			_cacheService = cacheService;
			_mapper = mapper;
			_createValidator = createValidator;
			_updateValidator = updateValidator;
		}

		public async Task<BookResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var cacheKey = BuildCacheKey(id);
			var cached = await _cacheService.GetAsync<BookResponse>(cacheKey, cancellationToken);
			if (cached is not null)
			{
				return cached;
			}
			var book = await _bookService.GetByIdAsync(id, cancellationToken);
			if (book is null)
			{
				return null;
			}
			var response = _mapper.Map<BookResponse>(book);
			await _cacheService.SetAsync(cacheKey, response, CacheDuration, cancellationToken);
			return response;
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
			await _cacheService.RemoveAsync(BuildCacheKey(id), cancellationToken);
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			await _bookService.DeleteAsync(id, cancellationToken);
			await _cacheService.RemoveAsync(BuildCacheKey(id), cancellationToken);

		}
		public async Task<Shared.Paging.PagedResult<BookResponse>> SearchAsync(BookSearchRequest request, CancellationToken cancellationToken = default)
		{
			var result = await _bookService.SearchAsync(request.Title, request.AuthorId, request.PublisherId, request.CategoryId, request.MinYear, request.MaxYear, request.OnlyAvailable, request.PageNumber, request.PageSize, cancellationToken);
			return _mapper.Map<Shared.Paging.PagedResult<BookResponse>>(result);
		}
		private static string BuildCacheKey(Guid id) => $"book:{id}";
	}
}
