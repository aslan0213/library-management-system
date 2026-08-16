using Xunit;
using Moq;
using Services;
using Abstractions.Repositories;
using Domain.Entities;
using Domain.Exceptions;
using Abstractions.Services;


namespace UnitTests
{
	public class LoanServiceTests
	{
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<IBookRepository> _bookRepositoryMock;
		private readonly Mock<IMemberRepository> _memberRepositoryMock;
		private readonly Mock<ILoanRepository> _loanRepositoryMock;
		private readonly Mock<IReservationService> _reservationServiceMock;
		private readonly LoanService _sut;

		public LoanServiceTests()
		{
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_bookRepositoryMock = new Mock<IBookRepository>();
			_memberRepositoryMock = new Mock<IMemberRepository>();
			_loanRepositoryMock = new Mock<ILoanRepository>();
			_reservationServiceMock = new Mock<IReservationService>();
			_unitOfWorkMock.Setup(u => u.Books).Returns(_bookRepositoryMock.Object);
			_unitOfWorkMock.Setup(u => u.Members).Returns(_memberRepositoryMock.Object);
			_unitOfWorkMock.Setup(u => u.Loans).Returns(_loanRepositoryMock.Object);
			_sut = new LoanService(_unitOfWorkMock.Object, _reservationServiceMock.Object);
		}

		[Fact]
		public async Task CreateLoanAsync_ThrowsNotFoundException_WhenBookDoesNotExist()
		{
			_bookRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((Book?)null);

			await Assert.ThrowsAsync<NotFoundException>(() =>
				_sut.CreateLoanAsync(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(14)));
		}

		[Fact]
		public async Task CreateLoanAsync_ThrowsNotFoundException_WhenMemberDoesNotExist()
		{
			var book = new Book { Id = Guid.NewGuid(), Title = "sen necede gozelsen", Isbn = "1234567890", PublisherId = Guid.NewGuid(), TotalCopies = 1, AvailableCopies = 1 };
			_bookRepositoryMock.Setup(r => r.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(book);
			_memberRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((Member?)null);

			await Assert.ThrowsAsync<NotFoundException>(() =>
				_sut.CreateLoanAsync(book.Id, Guid.NewGuid(), DateTime.UtcNow.AddDays(14)));
		}

		[Fact]
		public async Task CreateLoanAsync_ThrowsBusinessRuleViolationException_WhenNoAvailableCopies()
		{
			var book = new Book { Id = Guid.NewGuid(), Title = "sen necede gozelsen", Isbn = "1234567890", PublisherId = Guid.NewGuid(), TotalCopies = 1, AvailableCopies = 0 };
			var member = new Member { Id = Guid.NewGuid(), FirstName = "Aslan", LastName = "Mammadov", Email = "aslanmamedov@example.com" };
			_bookRepositoryMock.Setup(r => r.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(book);
			_memberRepositoryMock.Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(member);
			_unitOfWorkMock.Setup(u => u.Reservations.GetFulfilledForMemberAndBookAsync(member.Id, book.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync((Reservation?)null);

			await Assert.ThrowsAsync<BusinessRuleViolationException>(() =>
				_sut.CreateLoanAsync(book.Id, member.Id, DateTime.UtcNow.AddDays(14)));
		}

		[Fact]
		public async Task CreateLoanAsync_DecrementsAvailableCopiesAndCreatesLoan_WhenValid()
		{
			var book = new Book { Id = Guid.NewGuid(), Title = "sen necede gozelsen", Isbn = "1234567890", PublisherId = Guid.NewGuid(), TotalCopies = 3, AvailableCopies = 3 };
			var member = new Member { Id = Guid.NewGuid(), FirstName = "Aslan", LastName = "Mammadov", Email = "aslanmamedov@example.com" };
			var dueAt = DateTime.UtcNow.AddDays(14);

			_bookRepositoryMock.Setup(r => r.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(book);
			_memberRepositoryMock.Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(member);

			var result = await _sut.CreateLoanAsync(book.Id, member.Id, dueAt);

			Assert.Equal(2, book.AvailableCopies);
			Assert.Equal(book.Id, result.BookId);
			Assert.Equal(member.Id, result.MemberId);
			Assert.Equal(dueAt, result.DueAt);
			Assert.Null(result.ReturnedAt);
			_bookRepositoryMock.Verify(r => r.Update(book), Times.Once);
			_loanRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Loan>(), It.IsAny<CancellationToken>()), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task ReturnLoanAsync_ThrowsNotFoundException_WhenLoanDoesNotExist()
		{
			_loanRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((Loan?)null);

			await Assert.ThrowsAsync<NotFoundException>(() => _sut.ReturnLoanAsync(Guid.NewGuid()));
		}

		[Fact]
		public async Task ReturnLoanAsync_ThrowsBusinessRuleViolationException_WhenAlreadyReturned()
		{
			var loan = new Loan
			{
				Id = Guid.NewGuid(),
				BookId = Guid.NewGuid(),
				MemberId = Guid.NewGuid(),
				BorrowedAt = DateTime.UtcNow.AddDays(-10),
				DueAt = DateTime.UtcNow.AddDays(4),
				ReturnedAt = DateTime.UtcNow.AddDays(-1)
			};
			_loanRepositoryMock.Setup(r => r.GetByIdAsync(loan.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(loan);

			await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _sut.ReturnLoanAsync(loan.Id));
		}

		[Fact]
		public async Task ReturnLoanAsync_SetsReturnedAtAndCallsFulfillNextOrRelease_WhenValid()
		{
			var book = new Book { Id = Guid.NewGuid(), Title = "sen necede gozelsen", Isbn = "1234567890", PublisherId = Guid.NewGuid(), TotalCopies = 3, AvailableCopies = 1 };
			var loan = new Loan
			{
				Id = Guid.NewGuid(),
				BookId = book.Id,
				MemberId = Guid.NewGuid(),
				BorrowedAt = DateTime.UtcNow.AddDays(-5),
				DueAt = DateTime.UtcNow.AddDays(9),
				ReturnedAt = null
			};
			_loanRepositoryMock.Setup(r => r.GetByIdAsync(loan.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(loan);
			_bookRepositoryMock.Setup(r => r.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(book);

			await _sut.ReturnLoanAsync(loan.Id);

			Assert.NotNull(loan.ReturnedAt);
			_loanRepositoryMock.Verify(r => r.Update(loan), Times.Once);
			_reservationServiceMock.Verify(r => r.FulfillNextOrReleaseAsync(book.Id, It.IsAny<CancellationToken>()), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
