using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Persistence;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
namespace UnitTests
{
	public class NPlusOneProofTests
	{
		private static (LibraryDbContext context, List<string> ExecutedCommands) CreateContextWithQueryLogging()
		{
			var executedCommands = new List<string>();
			var loggerFactory = LoggerFactory.Create(builder =>
			{
				builder.AddProvider(new CapturingLoggerProvider(executedCommands));
			});
			var options = new DbContextOptionsBuilder<LibraryDbContext>()
				.UseSqlite("DataSource=:memory:")
				.UseLoggerFactory(loggerFactory)
				.EnableSensitiveDataLogging()
				.Options;
			var context = new LibraryDbContext(options);
			context.Database.OpenConnection();
			context.Database.EnsureCreated();
			return (context, executedCommands);
		}


		[Fact]
		public async Task GetPagedBooks_ExecutesExactlyOneQuery_NotOnePlusNForRelatedEntities()
		{
			var (context, executedCommands) = CreateContextWithQueryLogging();
			using var contextScope = context;

			var author = new Author { Id = Guid.NewGuid(), FirstName = "F", LastName = "L" };
			var publisher = new Publisher { Id = Guid.NewGuid(), Name = "P" };
			var category = new Category { Id = Guid.NewGuid(), Name = "Fiction" };

			context.Authors.Add(author);
			context.Publishers.Add(publisher);
			context.Categories.Add(category);

			for (var i = 0; i < 5; i++)
			{
				var book = new Book
				{
					Id = Guid.NewGuid(),
					Title = $"Book {i}",
					Isbn = $"ISBN-{i}",
					PublishedYear = 2000 + i,
					TotalCopies = 1,
					AvailableCopies = 1,
					AuthorId = author.Id,
					PublisherId = publisher.Id
				};
				book.Categories.Add(category);
				context.Books.Add(book);
			}
			await context.SaveChangesAsync();

			executedCommands.Clear();

			var repository = new BookRepository(context);
			var result = await repository.GetPagedAsync(new Abstractions.Paging.PagedQuery { PageNumber = 1, PageSize = 10 });

			foreach (var book in result.Items)
			{
				_ = book.Author.FirstName;
				_ = book.Publisher.Name;
				_ = book.Categories.Count;
			}

			var selectCommands = executedCommands.Count(c => c.Contains("SELECT", StringComparison.OrdinalIgnoreCase));

			Assert.Equal(5, result.Items.Count);
			Assert.True(
				selectCommands <= 2,
				$"Expected at most 2 SELECT statements (main paged query + count query), but {selectCommands} were executed. " +
				"This indicates an N+1 problem — related entities are being fetched with separate queries per row.");
		}


		private class CapturingLoggerProvider : ILoggerProvider
		{
			private readonly List<string> _commands;
			public CapturingLoggerProvider(List<string> commands)
			{
				_commands = commands;
			}
			public ILogger CreateLogger(string categoryName)
			{
				return new CapturingLogger(_commands);
			}
			public void Dispose()
			{
			}

			private class CapturingLogger : ILogger
			{
				private readonly List<string> _commands;
				public CapturingLogger(List<string> commands)
				{
					_commands = commands;
				}
				public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
				public bool IsEnabled(LogLevel logLevel) => logLevel == LogLevel.Information;
				public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
				{
					if (eventId.Id == Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuted.Id)
					{
						_commands.Add(formatter(state, exception));
					}
				}
			}
		}
	}
}
