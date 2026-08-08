using Abstractions.Repositories;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly LibraryDbContext _context;

		private IAuthorRepository? _authors;
		private IBookRepository? _books;
		private ILoanRepository? _loans;
		private IMemberRepository? _members;
		private IUserRepository? _users;
		private IRefreshTokenRepository? _refreshTokens;

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

		public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return await _context.SaveChangesAsync(cancellationToken);
		}
	}
}
