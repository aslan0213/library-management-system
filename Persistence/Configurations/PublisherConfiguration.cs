using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
	{
		public void Configure(EntityTypeBuilder<Publisher> builder)
		{
			builder.HasKey(p => p.Id);

			builder.Property(p => p.Name)
				.IsRequired()
				.HasMaxLength(150);

			builder.Property(p => p.Country)
				.HasMaxLength(100);

			builder.HasMany(p => p.Books)
				.WithOne(b => b.Publisher)
				.HasForeignKey(b => b.PublisherId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
