using Xunit;
using Moq;
using Services;
using Abstractions.Repositories;
using Domain.Entities;
using Domain.Exceptions;
namespace UnitTests
{
	public class BookServiceTests
	{
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<IBookRepository> _bookRepositoryMock;
		private readonly Mock<IAuthorRepository> _authorRepositoryMock;
		private readonly BookService _sut;

		public BookServiceTests()
		{
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_bookRepositoryMock = new Mock<IBookRepository>();
			_authorRepositoryMock = new Mock<IAuthorRepository>();
			_unitOfWorkMock.Setup(u => u.Books).Returns(_bookRepositoryMock.Object);
			_unitOfWorkMock.Setup(u => u.Authors).Returns(_authorRepositoryMock.Object);
			_sut = new BookService(_unitOfWorkMock.Object);
		}

		[Fact]
		public async Task CreateAsync_ThrowsNotFoundException_WhenAuthorDoesNotExist()
		{
			_authorRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((Author?)null);

			var book = new Book { Title = "Heyatdida Zaur", Isbn = "1234567890", Publisher = "Cavadin nesli", TotalCopies = 5, AuthorId = Guid.NewGuid() };

			await Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateAsync(book));
		}

		[Fact]
		public async Task CreateAsync_SetsAvailableCopiesEqualToTotalCopies_WhenAuthorExists()
		{
			var author = new Author { Id = Guid.NewGuid(), FirstName = "F", LastName = "L" };
			_authorRepositoryMock.Setup(r => r.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(author);

			var book = new Book { Title = "Heyatdida Zaur", Isbn = "1234567890", Publisher = "Cavadin nesli", TotalCopies = 5, AuthorId = author.Id };

			var result = await _sut.CreateAsync(book);

			Assert.Equal(5, result.AvailableCopies);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task DeleteAsync_ThrowsBusinessRuleViolationException_WhenCopiesAreOnLoan()
		{
			var existing = new Book
			{
				Id = Guid.NewGuid(),
				Title = "Heyatdida Zaur",
				Isbn = "1234567890",
				Publisher = "Cavadin nesli",
				TotalCopies = 5,
				AvailableCopies = 3
			};
			_bookRepositoryMock.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(existing);

			await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _sut.DeleteAsync(existing.Id));
		}

		[Fact]
		public async Task DeleteAsync_Succeeds_WhenAllCopiesAvailable()
		{
			var existing = new Book
			{
				Id = Guid.NewGuid(),
				Title = "Heyatdida Zaur",
				Isbn = "1234567890",
				Publisher = "Cavadin nesli",
				TotalCopies = 5,
				AvailableCopies = 5
			};
			_bookRepositoryMock.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(existing);

			await _sut.DeleteAsync(existing.Id);

			_bookRepositoryMock.Verify(r => r.Delete(existing), Times.Once);
		}

		[Fact]
		public async Task UpdateAsync_ThrowsBusinessRuleViolationException_WhenLoweringTotalCopiesBelowCopiesOnLoan()
		{
			var author = new Author { Id = Guid.NewGuid(), FirstName = "Fyodor", LastName = "Dostoevsky" };
			var existing = new Book
			{
				Id = Guid.NewGuid(),
				Title = "Heyatdida Zaur",
				Isbn = "1234567890",
				Publisher = "Cavadin nesli",
				TotalCopies = 5,
				AvailableCopies = 2, 
				AuthorId = author.Id
			};
			_bookRepositoryMock.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(existing);
			_authorRepositoryMock.Setup(r => r.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(author);

			var update = new Book
			{
				Id = existing.Id,
				Title = "Heyatdida Zaur",
				Isbn = "1234567890",
				Publisher = "Cavadin nesli",
				TotalCopies = 2, 
				AuthorId = author.Id
			};

			await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _sut.UpdateAsync(update));
		}
	}
}
