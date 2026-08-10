using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;
namespace Persistence.Configurations
{
	public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
	{
		public void Configure(EntityTypeBuilder<Notification> builder)
		{
			builder.HasKey(n => n.Id);

			builder.Property(n => n.Message)
				.IsRequired()
				.HasMaxLength(500);

			builder.Property(n => n.IsRead)
				.IsRequired()
				.HasDefaultValue(false);

			builder.Property(n => n.SentAt)
				.IsRequired();

			builder.HasOne(n => n.Member)
				.WithMany(u => u.Notifications)
				.HasForeignKey(n => n.MemberId)
				.OnDelete(DeleteBehavior.Cascade);
		} 
	}
}
