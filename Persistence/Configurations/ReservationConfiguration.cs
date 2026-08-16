using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
	public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
	{
		public void Configure(EntityTypeBuilder<Reservation> builder)
		{
			builder.HasKey(r => r.Id);

			builder.Property(r => r.ReservedAt)
				.IsRequired();

			builder.Property(r => r.Status)
				.IsRequired()
				.HasConversion<string>()
				.HasMaxLength(20);

			builder.HasOne(r => r.Book)
				.WithMany(b => b.Reservations)
				.HasForeignKey(r => r.BookId)
				.OnDelete(DeleteBehavior.Restrict);
			builder.HasOne(r => r.Member)
				.WithMany(m => m.Reservations)
				.HasForeignKey(r => r.MemberId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
