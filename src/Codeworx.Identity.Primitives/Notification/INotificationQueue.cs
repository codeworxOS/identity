using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Notification
{
    public interface INotificationQueue
    {
        Task EnqueueAsync(INotification notification, CancellationToken token = default);

        Task<INotification> DequeueAsync(CancellationToken token = default);
    }
}
