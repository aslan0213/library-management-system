using Abstractions.Repositories;
using Domain.Entities;
using Moq;
using Services.Reservations;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
	public class ReservationServiceRollbackTests
	{

		[Fact]
		public async Task CreateReservationAsync_RollsBackTransaction_WhenSecondSaveChangesFails()
		{
			var unitOfWorkMock = new Mock<IUnitOfWork>();
			var bookRepositoryMock = new Mock<IBookRepository>();
			var memberRepositoryMock = new Mock<IMemberRepository>();
			var reservationRepositoryMock = new Mock<IReservationRepository>();
			var notificationRepositoryMock = new Mock<INotificationRepository>();

			var book = new Book { Id = Guid.NewGuid(), Title = "Harry Potter", Isbn = "1234567890", PublisherId = Guid.NewGuid(), TotalCopies = 1, AvailableCopies = 0 };
			var member = new Member { Id = Guid.NewGuid(), FirstName = "Aslan", LastName = "Mammadov", Email = "aslanmammadov0213@gmail.com" };

			unitOfWorkMock.Setup(u => u.Books).Returns(bookRepositoryMock.Object);
			unitOfWorkMock.Setup(u => u.Members).Returns(memberRepositoryMock.Object);
			unitOfWorkMock.Setup(u => u.Reservations).Returns(reservationRepositoryMock.Object);
			unitOfWorkMock.Setup(u => u.Notifications).Returns(notificationRepositoryMock.Object);

			bookRepositoryMock.Setup(r => r.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(book);
			memberRepositoryMock.Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(member);
			reservationRepositoryMock.Setup(r => r.GetActiveForMemberAndBookAsync(member.Id, book.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync((Reservation?)null);

			unitOfWorkMock.SetupSequence(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(1)
				.ThrowsAsync(new InvalidOperationException("Simulated failure writing the notification."));

			var sut = new ReservationService(unitOfWorkMock.Object);

			await Assert.ThrowsAsync<InvalidOperationException>(() =>
				sut.CreateReservationAsync(book.Id, member.Id));

			unitOfWorkMock.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
			unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
			unitOfWorkMock.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
		}

		[Fact]
		public async Task CreateReservationAsync_CommitsTransaction_WhenBothWritesSucceed()
		{
			var unitOfWorkMock = new Mock<IUnitOfWork>();
			var bookRepositoryMock = new Mock<IBookRepository>();
			var memberRepositoryMock = new Mock<IMemberRepository>();
			var reservationRepositoryMock = new Mock<IReservationRepository>();
			var notificationRepositoryMock = new Mock<INotificationRepository>();

			var book = new Book { Id = Guid.NewGuid(), Title = "Harry Potter", Isbn = "1234567890", PublisherId = Guid.NewGuid(), TotalCopies = 1, AvailableCopies = 0 };
			var member = new Member { Id = Guid.NewGuid(), FirstName = "Aslan", LastName = "Mammadov", Email = "aslanmammadov0213@gmail.com" };

			unitOfWorkMock.Setup(u => u.Books).Returns(bookRepositoryMock.Object);
			unitOfWorkMock.Setup(u => u.Members).Returns(memberRepositoryMock.Object);
			unitOfWorkMock.Setup(u => u.Reservations).Returns(reservationRepositoryMock.Object);
			unitOfWorkMock.Setup(u => u.Notifications).Returns(notificationRepositoryMock.Object);

			bookRepositoryMock.Setup(r => r.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(book);
			memberRepositoryMock.Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(member);
			reservationRepositoryMock.Setup(r => r.GetActiveForMemberAndBookAsync(member.Id, book.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync((Reservation?)null);
			unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(1);

			var sut = new ReservationService(unitOfWorkMock.Object);

			var result = await sut.CreateReservationAsync(book.Id, member.Id);

			Assert.NotNull(result);
			unitOfWorkMock.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
			unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
		}
	}
}
