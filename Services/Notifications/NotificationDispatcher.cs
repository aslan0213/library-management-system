using Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Services.Notifications
{
	public class NotificationDispatcher : INotificationDispatcher
	{
		private readonly Channel<Guid> _channel;
		public NotificationDispatcher(Channel<Guid> channel)
		{
			_channel = channel;
		}
		public void QueueSend(Guid notificationId)
		{
			if(!_channel.Writer.TryWrite(notificationId))
			{
				throw new InvalidOperationException("Failed to queue notification for sending.");
			}
		}
	}
}
