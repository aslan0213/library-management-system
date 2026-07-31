using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.HasKey(u => u.Id);
			builder.Property(u => u.Email)
				.IsRequired()
				.HasMaxLength(256);
			builder.HasIndex(u => u.Email)
				.IsUnique();
			builder.Property(u => u.PasswordHash)
				.IsRequired();
			builder.Property(u=>u.role)
				.IsRequired() 
				.HasMaxLength(20)
				.HasConversion<string>();
			builder.Property(u => u.CreatedAt)
				.IsRequired();
			builder.HasMany(u => u.RefreshTokens)
				.WithOne(rt => rt.User)
				.HasForeignKey(rt => rt.UserId)
				.OnDelete(DeleteBehavior.Cascade);



		}
	}
}
