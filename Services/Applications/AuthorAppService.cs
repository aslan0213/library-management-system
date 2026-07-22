using Shared.Dtos.Author;
using Shared.Paging;
using Abstractions.Services;
using Abstractions.Paging;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Services.Applications
{
	public class AuthorAppService : IAuthorAppService
	{
		private readonly IAuthorService _authorService;
		private readonly IMapper _mapper;
		private readonly IValidator<CreateAuthorRequest> _createValidator;
		private readonly IValidator<UpdateAuthorRequest> _updateValidator;
		public AuthorAppService(IAuthorService authorService, IMapper mapper, IValidator<CreateAuthorRequest> createValidator, IValidator<UpdateAuthorRequest> updateValidator)
		{
			_authorService = authorService;
			_mapper = mapper;
			_createValidator = createValidator;
			_updateValidator = updateValidator;
		}
		public async Task<AuthorResponce> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default)
		{
			await _createValidator.ValidateAndThrowAsync(request,cancellationToken);
			var author = _mapper.Map<Domain.Entities.Author>(request);
			var createdAuthor = await _authorService.CreateAsync(author, cancellationToken);
			return _mapper.Map<AuthorResponce>(createdAuthor);
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			await _authorService.DeleteAsync(id, cancellationToken);
		}

		public async Task<AuthorResponce?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var author = await _authorService.GetByIdAsync(id, cancellationToken);
			return author is null ? null: _mapper.Map<AuthorResponce>(author);
		}

		public async Task<Shared.Paging.PagedResult<AuthorResponce>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
		{
			var query = _mapper.Map<PagedQuery>(request);
			var authors = await _authorService.GetPagedAsync(query, cancellationToken);
			return _mapper.Map<Shared.Paging.PagedResult<AuthorResponce>>(authors);
		}

		public async Task UpdateAsync(Guid id, UpdateAuthorRequest request, CancellationToken cancellationToken = default)
		{
			await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);
			var author = _mapper.Map<Domain.Entities.Author>(request);
			author.Id = id;
			await _authorService.UpdateAsync(author, cancellationToken);
		}
	}
}
