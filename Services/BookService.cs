using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Paging;
using Abstractions.Repositories;
using Abstractions.Services;
using Domain.Entities;
using Domain.Exceptions;
using Services.Specifications;
namespace Services
{
	public class BookService : IBookService
	{
		private readonly IUnitOfWork _unitOfWork;
		public BookService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public async Task<Book> CreateAsync(Book book, List<Guid> categoryIds, CancellationToken cancellationToken = default)
		{
			var author = await _unitOfWork.Authors.GetByIdAsync(book.AuthorId, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Author), book.AuthorId);
			var publisher = await _unitOfWork.Publishers.GetByIdAsync(book.PublisherId, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Publisher), book.PublisherId);
			var categories = await ResolveCategoriesAsync(categoryIds, cancellationToken);
			book.Id = Guid.NewGuid();
			book.AvailableCopies = book.TotalCopies;
			foreach (var category in categories)
			{
				book.Categories.Add(category);
			}
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

		public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
			await _unitOfWork.Books.GetByIdAsync(id, cancellationToken);


		public async Task<PagedResult<Book>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default) =>
			await _unitOfWork.Books.GetPagedAsync(query, cancellationToken);



		public async Task UpdateAsync(Book book, List<Guid> categoryIds, CancellationToken cancellationToken = default)
		{
			var existing = await _unitOfWork.Books.GetByIdAsync(book.Id, cancellationToken)
				?? throw NotFoundException.ForEntity(nameof(Book), book.Id);
			if (!await AuthorExistsAsync(book.AuthorId, cancellationToken))
			{
				throw NotFoundException.ForEntity(nameof(Author), book.AuthorId);
			}
			if (await _unitOfWork.Publishers.GetByIdAsync(book.PublisherId, cancellationToken) is null)
			{
				throw NotFoundException.ForEntity(nameof(Publisher), book.PublisherId);
			}
			var copiesOnLoan = existing.TotalCopies - existing.AvailableCopies;
			if (book.TotalCopies < copiesOnLoan)
			{
				throw new BusinessRuleViolationException(
					$"Cannot set TotalCopies to {book.TotalCopies}; {copiesOnLoan} copies are currently on loan.");
			}
			var categories = await ResolveCategoriesAsync(categoryIds, cancellationToken);
			existing.Title = book.Title;
			existing.Isbn = book.Isbn;
			existing.PublishedYear = book.PublishedYear;
			existing.TotalCopies = book.TotalCopies;
			existing.AvailableCopies = existing.AvailableCopies + (book.TotalCopies - existing.TotalCopies);
			existing.AuthorId = book.AuthorId;
			existing.PublisherId = book.PublisherId;
			existing.Categories.Clear();
			foreach (var category in categories)
			{
				existing.Categories.Add(category);
			}
			_unitOfWork.Books.Update(existing);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
		public async Task<bool> AuthorExistsAsync(Guid authorId, CancellationToken cancellationToken = default) =>
			await _unitOfWork.Authors.GetByIdAsync(authorId, cancellationToken) is not null;

		private async Task<List<Category>> ResolveCategoriesAsync(List<Guid> categoryIds, CancellationToken cancellationToken)
		{
			var categories = new List<Category>();
			foreach (var categoryId in categoryIds)
			{
				var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken)
					?? throw NotFoundException.ForEntity(nameof(Category), categoryId);
				categories.Add(category);
			}
			return categories;

		}

		public async Task<PagedResult<Book>> SearchAsync(string? title, Guid? authorId, Guid? publisherId, Guid? categoryId, int? minYear, int? maxYear, bool? onlyAvailable, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
		{
			var specification = new BookSearchSpecification(title, authorId, publisherId, categoryId, minYear, maxYear, onlyAvailable);
			return await _unitOfWork.Books.SearchAsync(specification, pageNumber, pageSize, cancellationToken);
		}
	}
}
