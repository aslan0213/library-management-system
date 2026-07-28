using Abstractions.Services;
using Abstractions.Paging;
using AutoMapper;
using FluentValidation;
using Shared.Dtos.Member;
using Shared.Paging;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Applications
{
	public class MemberAppService : IMemberAppService
	{
		private readonly IMemberService _memberService;
		private readonly IMapper _mapper;
		private readonly IValidator<CreateMemberRequest> _createValidator;
		private readonly IValidator<UpdateMemberRequest> _updateValidator;

		public MemberAppService(
			IMemberService memberService,
			IMapper mapper,
			IValidator<CreateMemberRequest> createValidator,
			IValidator<UpdateMemberRequest> updateValidator)
		{
			_memberService = memberService;
			_mapper = mapper;
			_createValidator = createValidator;
			_updateValidator = updateValidator;
		}

		public async Task<MemberResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var member = await _memberService.GetByIdAsync(id, cancellationToken);
			return member is null ? null : _mapper.Map<MemberResponse>(member);
		}

		public async Task<Shared.Paging.PagedResult<MemberResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
		{
			var query = _mapper.Map<PagedQuery>(request);
			var result = await _memberService.GetPagedAsync(query, cancellationToken);
			return _mapper.Map<Shared.Paging.PagedResult<MemberResponse>>(result);
		}

		public async Task<MemberResponse> CreateAsync(CreateMemberRequest request, CancellationToken cancellationToken = default)
		{
			await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

			var member = _mapper.Map<Member>(request);
			var created = await _memberService.CreateAsync(member, cancellationToken);
			return _mapper.Map<MemberResponse>(created);
		}

		public async Task UpdateAsync(Guid id, UpdateMemberRequest request, CancellationToken cancellationToken = default)
		{
			await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

			var member = _mapper.Map<Member>(request);
			member.Id = id;
			await _memberService.UpdateAsync(member, cancellationToken);
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
			await _memberService.DeleteAsync(id, cancellationToken);
	}
}
