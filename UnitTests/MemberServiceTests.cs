using Abstractions.Repositories;
using Domain.Exceptions;
using Moq;
using Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
	public class MemberServiceTests
	{
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<IMemberRepository> _memberRepositoryMock;
		private readonly MemberService _sut;

		public MemberServiceTests()
		{
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_memberRepositoryMock = new Mock<IMemberRepository>();
			_unitOfWorkMock.Setup(u => u.Members).Returns(_memberRepositoryMock.Object);
			_sut = new MemberService(_unitOfWorkMock.Object);
		}

		[Fact]
		public async Task CreateAsync_SetsMembershipDateAndSaves()
		{
			var member = new Member { FirstName = "Aslan", LastName = "Mammadov", Email = "aslanmamedov@example.com" };
			var before = DateTime.UtcNow;

			var result = await _sut.CreateAsync(member);

			Assert.NotEqual(Guid.Empty, result.Id);
			Assert.True(result.MembershipDate >= before);
			_memberRepositoryMock.Verify(r => r.AddAsync(member, It.IsAny<CancellationToken>()), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task UpdateAsync_ThrowsNotFoundException_WhenMemberDoesNotExist()
		{
			_memberRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((Member?)null);

			var member = new Member { Id = Guid.NewGuid(), FirstName = "Razil", LastName = "Cavadov", Email = "razil155@example.com" };

			await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(member));
		}

		[Fact]
		public async Task DeleteAsync_DeletesAndSaves_WhenMemberExists()
		{
			var existing = new Member { Id = Guid.NewGuid(), FirstName = "Onur", LastName = "Mehemmed", Email = "onur03@example.com" };
			_memberRepositoryMock.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(existing);

			await _sut.DeleteAsync(existing.Id);

			_memberRepositoryMock.Verify(r => r.Delete(existing), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
