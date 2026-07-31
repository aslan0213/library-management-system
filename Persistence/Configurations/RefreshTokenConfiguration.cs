using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Persistence.Configurations
{
	public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
	{
		public void Configure(EntityTypeBuilder<RefreshToken> builder)
		{
			builder.HasKey(rt => rt.Id);
			builder.Property(rt => rt.TokenHash)
				.IsRequired()
				.HasMaxLength(256);
			builder.HasIndex(rt => rt.TokenHash)
				.IsUnique();
			builder.Property(rt => rt.ExpiresAt)
				.IsRequired();
			builder.Property(rt => rt.CreatedAt)
				.IsRequired();
			builder.Ignore(rt => rt.IsActive);
		}
	}
}
