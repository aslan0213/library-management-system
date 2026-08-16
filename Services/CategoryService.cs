using Abstractions.Paging;
using Abstractions.Repositories;
using Abstractions.Services;
using Domain.Entities;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
namespace Services
{
	public class CategoryService : ICategoryService
	{
		private readonly IUnitOfWork _unitOfWork;

		public CategoryService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<Category?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default) =>
			await _unitOfWork.Categories.GetByIdAsync(Id, cancellationToken);

		public async Task<PagedResult<Category>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default) =>
			await _unitOfWork.Categories.GetPagedAsync(query, cancellationToken);

		public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
		{
			category.Id = Guid.NewGuid();
			await _unitOfWork.Categories.AddAsync(category, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return category;
		}

		public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
		{
			var existing = await _unitOfWork.Categories.GetByIdAsync(category.Id, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Category), category.Id);

			existing.Name = category.Name;

			_unitOfWork.Categories.Update(existing);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var existing = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Category), id);

			_unitOfWork.Categories.Delete(existing);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
	}
}
