using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;
using Abstractions.Services;
using Abstractions.Repositories;
using Domain.Entities;
using Domain.Exceptions;

namespace Services
{
	public class PublisherService : IPublisherService
	{
		private readonly IUnitOfWork _unitOfWork;
		public PublisherService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<Publisher?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
			await _unitOfWork.Publishers.GetByIdAsync(id, cancellationToken);


		public async Task<PagedResult<Publisher>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default) =>
			await _unitOfWork.Publishers.GetPagedAsync(query, cancellationToken);


		public async Task<Publisher> CreateAsync(Publisher publisher, CancellationToken cancellationToken = default)
		{
			publisher.Id = Guid.NewGuid();
			await _unitOfWork.Publishers.AddAsync(publisher, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return publisher;
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var existing = await _unitOfWork.Publishers.GetByIdAsync(id, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Publisher), id);
		    _unitOfWork.Publishers.Delete(existing);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}



		public async Task UpdateAsync(Publisher publisher, CancellationToken cancellationToken = default)
		{
			var existing = await _unitOfWork.Publishers.GetByIdAsync(publisher.Id, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Publisher), publisher.Id);
			existing.Name = publisher.Name;
			existing.Country = publisher.Country;
			_unitOfWork.Publishers.Update(existing);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
	}
}
