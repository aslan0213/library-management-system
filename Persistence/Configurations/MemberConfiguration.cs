using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	public class MemberConfiguration : IEntityTypeConfiguration<Member>
	{
		public void Configure(EntityTypeBuilder<Member> builder)
		{
			builder.HasKey(m => m.Id);
			builder.Property(m => m.FirstName)
				.IsRequired()
				.HasMaxLength(100);
			builder.Property(m => m.LastName) 
				.IsRequired()
				.HasMaxLength(100);
			builder.Property(m => m.Email)
				.IsRequired()
				.HasMaxLength(256);
			builder.HasIndex(m => m.Email)
				.IsUnique();
			builder.Property(m => m.PhoneNumber)
				.HasMaxLength(20);
			builder.HasMany(m => m.Loans)
				.WithOne(l => l.Member)
				.HasForeignKey(l => l.MemberId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
