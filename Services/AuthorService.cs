using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;
using Abstractions.Repositories;
using Abstractions.Services;
using Domain.Entities;
using Domain.Exceptions;
namespace Services
{
	public class AuthorService : IAuthorService
	{
		private readonly IUnitOfWork _unitOfWork;
		public AuthorService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public Task<PagedResult<Author>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)=>
			_unitOfWork.Authors.GetPagedAsync(query, cancellationToken);

		public async Task<Author?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)=>
			await _unitOfWork.Authors.GetByIdAsync(id, cancellationToken);

		public async Task<Author> CreateAsync(Author author, CancellationToken cancellationToken = default)
		{
			author.Id = Guid.NewGuid();
			await _unitOfWork.Authors.AddAsync(author, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return author;
		}
		public async Task UpdateAsync(Author author, CancellationToken cancellationToken = default)
		{
			var existing = await _unitOfWork.Authors.GetByIdAsync(author.Id, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Author), author.Id);
			existing.FirstName = author.FirstName;
			existing.LastName = author.LastName;
			existing.Bio = author.Bio;
			existing.DateOfBirth = author.DateOfBirth;
			_unitOfWork.Authors.Update(existing);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var existing = await _unitOfWork.Authors.GetByIdAsync(id, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Author), id);
			_unitOfWork.Authors.Delete(existing);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
	}
}
