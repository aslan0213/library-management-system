using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Abstractions.Repositories;
namespace Persistence.Repositories
{
	public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
	{
		public NotificationRepository(LibraryDbContext context) : base(context)
		{
		}
	}
}
