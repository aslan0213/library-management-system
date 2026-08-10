using Abstractions.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly LibraryDbContext _context;
		private IDbContextTransaction? _currentTransaction;

		private IAuthorRepository? _authors;
		private IBookRepository? _books;
		private ILoanRepository? _loans;
		private IMemberRepository? _members;
		private IUserRepository? _users;
		private IRefreshTokenRepository? _refreshTokens;
		private IPublisherRepository? _publishers;
		private ICategoryRepository? _categories;
		private INotificationRepository? _notifications;
		private IReservationRepository? _reservations;


		public UnitOfWork(LibraryDbContext context)
		{
			_context = context;
		}

		public IAuthorRepository Authors => _authors??= new AuthorRepository(_context);
		public IBookRepository Books => _books??= new BookRepository(_context);
		public ILoanRepository Loans => _loans??= new LoanRepository(_context);
		public IMemberRepository Members => _members??= new MemberRepository(_context);
		public IUserRepository Users =>_users ??= new UserRepository(_context);
		public IRefreshTokenRepository RefreshTokens =>_refreshTokens ??= new RefreshTokenRepository(_context);
		public INotificationRepository Notifications => _notifications ??= new NotificationRepository(_context);
		public IReservationRepository Reservations => _reservations ??= new ReservationRepository(_context);
		public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
		public IPublisherRepository Publishers => _publishers ??= new PublisherRepository(_context);

		public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return await _context.SaveChangesAsync(cancellationToken);
		}

		public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
		{
			_currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
		}

		public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
		{
			if(_currentTransaction is null)
			{
				throw new InvalidOperationException("No active transaction to commit.");
			}
			
			await _currentTransaction.CommitAsync(cancellationToken);
			await _currentTransaction.DisposeAsync();
			_currentTransaction = null;
		}

		public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
		{
			if(_currentTransaction is null)
			{
				return;
			}
			await  _currentTransaction.RollbackAsync(cancellationToken);
			await _currentTransaction.DisposeAsync();
			_currentTransaction = null;
		}


	}
}
