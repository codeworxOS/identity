using Codeworx.Identity.Model;

namespace Codeworx.Identity.Notification
{
    public interface INotification
    {
        string Subject { get; }

        string TemplateKey { get; }

        IUser Target { get; }
    }
}