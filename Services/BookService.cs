using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;
using Abstractions.Repositories;
using Abstractions.Services;
using Domain.Entities;
using Domain.Exceptions;
namespace Services
{
	public class BookService : IBookService
	{
		private readonly IUnitOfWork _unitOfWork;
		public BookService(IUnitOfWork _unitOfWork)
		{
			_unitOfWork = _unitOfWork;
		}
		public async Task<Book> CreateAsync(Book book, CancellationToken cancellationToken = default)
		{
			var author = await _unitOfWork.Authors.GetByIdAsync(book.AuthorId, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Author), book.AuthorId);
			book.Id = Guid.NewGuid();
			book.AvailableCopies = book.TotalCopies;
			await _unitOfWork.Books.AddAsync(book, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return book;
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var existing = await _unitOfWork.Books.GetByIdAsync(id, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Book), id);
			if (existing.AvailableCopies != existing.TotalCopies)
			{
				throw new BusinessRuleViolationException("Cannot delete a book that currently has copies on loan.");
			}
			_unitOfWork.Books.Delete(existing);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)=>
			await _unitOfWork.Books.GetByIdAsync(id, cancellationToken);


		public async Task<PagedResult<Book>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)=>
			await _unitOfWork.Books.GetPagedAsync(query, cancellationToken);



		public async Task UpdateAsync(Book book, CancellationToken cancellationToken = default)
		{
			var existing = await _unitOfWork.Books.GetByIdAsync(book.Id, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Book), book.Id);
			if (!await AuthorExistsAsync(book.AuthorId, cancellationToken))
			{
				throw NotFoundException.ForEntity(nameof(Author), book.AuthorId);
			}
			var copiesOnLoan = existing.TotalCopies - existing.AvailableCopies;
			if (book.TotalCopies < copiesOnLoan)
			{
				throw new BusinessRuleViolationException(
					"Cannot set TotalCopies to {book.TotalCopies}; {copiesOnLoan} copies are currently on loan.");
			}
			existing.Title = book.Title;
			existing.Isbn = book.Isbn;
			existing.PublishedYear = book.PublishedYear;
			existing.Publisher = book.Publisher;
			existing.TotalCopies = book.TotalCopies;
			existing.AvailableCopies = existing.AvailableCopies + (book.TotalCopies - existing.TotalCopies);
			existing.AuthorId = book.AuthorId;
			_unitOfWork.Books.Update(existing);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
		public async Task<bool> AuthorExistsAsync(Guid authorId, CancellationToken cancellationToken = default)=>
			await _unitOfWork.Authors.GetByIdAsync(authorId, cancellationToken) is not null;

	}
}
