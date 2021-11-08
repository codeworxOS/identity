using System.Threading.Tasks;

namespace Codeworx.Identity.Notification
{
    public interface INotificationService
    {
        Task SendNotificationAsync(INotification notification);

        Task<bool> IsSupportedAsync();
    }
}
