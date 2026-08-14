using Abstractions.Paging;
using Abstractions.Services;
using AutoMapper;
using FluentValidation;
using Shared.Dtos.Reservation;
using Shared.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public class ReservationAppService : IReservationAppService
	{
		private readonly IReservationService _reservationService;
		private readonly IMapper _mapper;
		private readonly IValidator<CreateReservationRequest> _createValidator;

		public ReservationAppService(IReservationService reservationService, IMapper mapper, IValidator<CreateReservationRequest> createValidator)
		{
			_reservationService = reservationService;
			_mapper = mapper;
			_createValidator = createValidator;
		}
		public async Task CancelReservationAsync(Guid reservationId, CancellationToken cancellationToken = default)
		{
			await _reservationService.CancelReservationAsync(reservationId, cancellationToken);
		}

		public async Task<ReservationResponse> CreateReservationAsync(CreateReservationRequest request, CancellationToken cancellationToken = default)
		{
			await _createValidator.ValidateAndThrowAsync(request, cancellationToken);
			var created = await _reservationService.CreateReservationAsync(request.BookId, request.MemberId, cancellationToken);
			return _mapper.Map<ReservationResponse>(created);
		}

		public async Task<ReservationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var reservation = await _reservationService.GetByIdAsync(id, cancellationToken);
			return reservation is null ? null : _mapper.Map<ReservationResponse>(reservation);
		}

		public async Task<Shared.Paging.PagedResult<ReservationResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
		{
			var query = _mapper.Map<PagedQuery>(request);
			var result = await _reservationService.GetPagedAsync(query, cancellationToken);
			return _mapper.Map<Shared.Paging.PagedResult<ReservationResponse>>(result);
		}
	}
}
