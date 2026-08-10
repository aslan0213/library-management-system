using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Persistence.Configurations
{
	public class BookConfiguration : IEntityTypeConfiguration<Book>
	{
		public void Configure(EntityTypeBuilder<Book> builder)
		{
			builder.HasKey(b => b.Id);

			builder.Property(b => b.Title)
				.IsRequired()
				.HasMaxLength(200);
			builder.Property(b => b.Isbn)
				.IsRequired()
				.HasMaxLength(13);
			builder.HasIndex(b => b.Isbn)
				.IsUnique();
			builder.HasMany(b => b.Loans)
				.WithOne(l => l.Book)
				.HasForeignKey(l => l.BookId)
				.OnDelete(DeleteBehavior.Restrict);
			builder.ToTable(t=>t.HasCheckConstraint(
				"CK_Book_AvailableCopies_LessThanOrEqualTotal",
				"\"AvailableCopies\" <= \"TotalCopies\""));
			builder.ToTable(t=>t.HasCheckConstraint(
				"CK_Book_AvailableCopies_NonNegative",
				"\"AvailableCopies\" >= 0"));
			builder.ToTable(t => t.HasCheckConstraint(
				"CK_Book_TotalCopies_NonNegative",
				"\"TotalCopies\" >= 0"));
		}
	}
}
