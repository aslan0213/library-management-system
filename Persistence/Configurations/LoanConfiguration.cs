using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	public class LoanConfiguration : IEntityTypeConfiguration<Loan>
	{
		public void Configure(EntityTypeBuilder<Loan> builder)
		{
			builder.HasKey(l => l.Id);
			builder.Property(l => l.BorrowedAt)
				.IsRequired();
			builder.Property(l=>l.DueAt)
				.IsRequired();
		}
	}
}
