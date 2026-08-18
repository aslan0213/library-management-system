using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions.Services
{
	public interface INotificationDispatcher
	{
		void QueueSend(Guid notificationId);
	}
}
