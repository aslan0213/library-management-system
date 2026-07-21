using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
namespace Persistence
{
	public class LibraryDbContext : DbContext
	{
		public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
		{
		}

		public DbSet<Author> Authors => Set<Author>();
		public DbSet<Book> Books => Set<Book>();
		public DbSet<Member> Members => Set<Member>();
		public DbSet<Loan> Loans => Set<Loan>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
		}
	}
}
