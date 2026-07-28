using System;
using System.Collections.Generic;
using System.Text;
using Services;
using Abstractions.Repositories;
using Domain.Entities;
using Domain.Exceptions;
using Xunit;
using Moq;
namespace UnitTests
{
	public class AuthorServiceTests
	{
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<IAuthorRepository> _authorRepositoryMock;
		private readonly AuthorService _sut;

		public AuthorServiceTests()
		{
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_authorRepositoryMock = new Mock<IAuthorRepository>();
			_unitOfWorkMock.Setup(u => u.Authors).Returns(_authorRepositoryMock.Object);
			_sut = new AuthorService(_unitOfWorkMock.Object);
		}

		[Fact]
		public async Task GetByIdAsync_ReturnsAuthor_WhenExists()
		{
			var author = new Author { Id = Guid.NewGuid(), FirstName = "Mikhail", LastName = "Lermontov" };
			_authorRepositoryMock.Setup(r => r.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(author);

			var result = await _sut.GetByIdAsync(author.Id);

			Assert.NotNull(result);
			Assert.Equal(author.Id, result!.Id);
		}

		[Fact]
		public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
		{
			_authorRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((Author?)null);

			var result = await _sut.GetByIdAsync(Guid.NewGuid());

			Assert.Null(result);
		}

		[Fact]
		public async Task CreateAsync_AssignsIdAndSaves()
		{
			var author = new Author { FirstName = "Mikhail", LastName = "Lermontov" };

			var result = await _sut.CreateAsync(author);

			Assert.NotEqual(Guid.Empty, result.Id);
			_authorRepositoryMock.Verify(r => r.AddAsync(author, It.IsAny<CancellationToken>()), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task UpdateAsync_ThrowsNotFoundException_WhenAuthorDoesNotExist()
		{
			_authorRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((Author?)null);

			var author = new Author { Id = Guid.NewGuid(), FirstName = "Fyodor", LastName = "Dostoevsky" };

			await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(author));
		}

		[Fact]
		public async Task UpdateAsync_UpdatesTrackedFields_WhenAuthorExists()
		{
			var existing = new Author { Id = Guid.NewGuid(), FirstName = "Old", LastName = "Old" };
			_authorRepositoryMock.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(existing);

			var updated = new Author { Id = existing.Id, FirstName = "New", LastName = "New" };

			await _sut.UpdateAsync(updated);

			Assert.Equal("New", existing.FirstName);
			Assert.Equal("New", existing.LastName);
			_authorRepositoryMock.Verify(r => r.Update(existing), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task DeleteAsync_ThrowsNotFoundException_WhenAuthorDoesNotExist()
		{
			_authorRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((Author?)null);

			await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(Guid.NewGuid()));
		}

		[Fact]
		public async Task DeleteAsync_DeletesAndSaves_WhenAuthorExists()
		{
			var existing = new Author { Id = Guid.NewGuid(), FirstName = "Alexandr", LastName = "Pushkin" };
			_authorRepositoryMock.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(existing);

			await _sut.DeleteAsync(existing.Id);

			_authorRepositoryMock.Verify(r => r.Delete(existing), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
