using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;
using Abstractions.Services;
using Domain.Entities;
using Abstractions.Repositories;
using Domain.Exceptions;
namespace Services
{
	public class MemberService : IMemberService
	{
		private readonly IUnitOfWork _unitOfWork;

		public MemberService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<Member> CreateAsync(Member member, CancellationToken cancellationToken = default)
		{
			member.Id = Guid.NewGuid();
			member.MembershipDate = DateTime.UtcNow;
			await _unitOfWork.Members.AddAsync(member, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return member;
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var existing = await _unitOfWork.Members.GetByIdAsync(id, cancellationToken)
				?? throw  NotFoundException.ForEntity(nameof(Member), id);
			_unitOfWork.Members.Delete(existing);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		public async Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)=>
			await _unitOfWork.Members.GetByIdAsync(id, cancellationToken);


		public async Task<PagedResult<Member>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)=>
			await _unitOfWork.Members.GetPagedAsync(query, cancellationToken);

		public async Task UpdateAsync(Member member, CancellationToken cancellationToken = default)
		{
			var existing = await _unitOfWork.Members.GetByIdAsync(member.Id, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Member), member.Id);
			existing.FirstName = member.FirstName;
			existing.LastName = member.LastName;
			existing.Email = member.Email;
			existing.PhoneNumber = member.PhoneNumber;
			_unitOfWork.Members.Update(existing);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
	}
}
