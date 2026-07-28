using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Configurations
{
	public class AuthorConfiguration : IEntityTypeConfiguration<Author>
	{
		public void Configure(EntityTypeBuilder<Author> builder)
		{
			builder.HasKey(a => a.Id);
			builder.Property(a=>a.FirstName)
				.IsRequired()
				.HasMaxLength(100);
			builder.Property(a=>a.LastName)
				.IsRequired()
				.HasMaxLength(100);
			builder.Property(a => a.Bio)
				.HasMaxLength(1000);
			builder.HasMany(a => a.Books)
				.WithOne(b => b.Author)
				.OnDelete(DeleteBehavior.Restrict);

		}
	}
}
