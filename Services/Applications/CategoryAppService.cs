using Abstractions.Services;
using AutoMapper;
using FluentValidation;
using Shared.Dtos.Category;
using Shared.Paging;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public class CategoryAppService : ICategoryAppService
	{
		private readonly ICategoryService _categoryService;
		private readonly IMapper _mapper;
		private readonly IValidator<CreateCategoryRequest> _createValidator;
		private readonly IValidator<UpdateCategoryRequest> _updateValidator;

		public CategoryAppService(ICategoryService categoryService, IMapper mapper, IValidator<CreateCategoryRequest> createValidator, IValidator<UpdateCategoryRequest> updateValidator)
		{
			_categoryService = categoryService;
			_mapper = mapper;
			_createValidator = createValidator;
			_updateValidator = updateValidator;
		}
		public async Task<CategoryResponse?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
		{
			var category = await _categoryService.GetByIdAsync(categoryId, cancellationToken);
			return category is null ? null : _mapper.Map<CategoryResponse>(category);
		}

		public async Task<PagedResult<CategoryResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
		{
			var query = _mapper.Map<Abstractions.Paging.PagedQuery>(request);
			var categories = await _categoryService.GetPagedAsync(query, cancellationToken);
			return _mapper.Map<PagedResult<CategoryResponse>>(categories);
		}

		public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
		{
			await _createValidator.ValidateAndThrowAsync(request, cancellationToken);
			var category = _mapper.Map<Category>(request);
			var created = await _categoryService.CreateAsync(category, cancellationToken);
			return _mapper.Map<CategoryResponse>(created);
		}

		public async Task DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default)
		{
			await _categoryService.DeleteAsync(categoryId, cancellationToken);
		}


		public async Task UpdateAsync(Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
		{
			await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);
			var category = _mapper.Map<Category>(request);
			category.Id = categoryId;
			await _categoryService.UpdateAsync(category, cancellationToken);
		}
	}
}
