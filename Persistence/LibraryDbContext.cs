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
		public DbSet<User> Users => Set<User>();
		public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
		public DbSet<Publisher> Publishers => Set<Publisher>();
		public DbSet<Category> Categories => Set<Category>();
		public DbSet<Reservation> Reservations => Set<Reservation>();
		public DbSet<Notification> Notifications => Set<Notification>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
		}
	}
}
