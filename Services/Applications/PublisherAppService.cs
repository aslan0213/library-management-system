using Shared.Dtos.Publisher;
using Shared.Paging;
using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Services;
using AutoMapper;
using FluentValidation;
using Domain.Entities;

namespace Services.Applications
{
	public class PublisherAppService : IPublisherAppService
	{
		private readonly IPublisherService _publisherService;
		private readonly IMapper _mapper;
		private readonly IValidator<CreatePublisherRequest> _createValidator;
		private readonly IValidator<UpdatePublisherRequest> _updateValidator;
		public PublisherAppService(IPublisherService publisherService, IMapper mapper, IValidator<CreatePublisherRequest> createValidator, IValidator<UpdatePublisherRequest> updateValidator)
		{
			_publisherService = publisherService;
			_mapper = mapper;
			_createValidator = createValidator;
			_updateValidator = updateValidator;
		}

		public async Task<PublisherResponse?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default)
		{
			var publisher = await _publisherService.GetByIdAsync(Id, cancellationToken);
			return publisher is null ? null : _mapper.Map<PublisherResponse>(publisher);
		}

		public async Task<PagedResult<PublisherResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
		{
			var pagedQuery = _mapper.Map<Abstractions.Paging.PagedQuery>(request);
			var result = await _publisherService.GetPagedAsync(pagedQuery, cancellationToken);
			return _mapper.Map<PagedResult<PublisherResponse>>(result);
		}

		public async Task<PublisherResponse> CreateAsync(CreatePublisherRequest request, CancellationToken cancellationToken = default)
		{
			await _createValidator.ValidateAndThrowAsync(request, cancellationToken);	
			var publisher = _mapper.Map<Publisher>(request);
			var created = await _publisherService.CreateAsync(publisher, cancellationToken);
			return _mapper.Map<PublisherResponse>(created);

		}

		public async Task UpdateAsync(Guid Id, UpdatePublisherRequest request, CancellationToken cancellationToken = default)
		{
			await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);
			var publisher = _mapper.Map<Publisher>(request);
			publisher.Id = Id;
			await _publisherService.UpdateAsync(publisher, cancellationToken);
		}

		public async Task DeleteAsync(Guid Id, CancellationToken cancellationToken = default)
		{
			await _publisherService.DeleteAsync(Id, cancellationToken);
		}
	}
}
