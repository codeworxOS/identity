using System.Threading.Tasks;

namespace Codeworx.Identity.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationQueue _queue;

        public NotificationService(INotificationQueue queue = null)
        {
            _queue = queue;
        }

        public Task<bool> IsSupportedAsync()
        {
            return Task.FromResult(_queue != null);
        }

        public async Task SendNotificationAsync(INotification notification)
        {
            await _queue.EnqueueAsync(notification);
        }
    }
}
