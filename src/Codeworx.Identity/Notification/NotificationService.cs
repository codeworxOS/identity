using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Mail;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly ITemplateCompiler _templateCompiler;
        private readonly IMailConnector _connector;
        private readonly ConcurrentDictionary<string, Func<object, string>> _notificationTemplateCache;

        public NotificationService(ITemplateCompiler templateCompiler, IMailConnector connector = null)
        {
            _templateCompiler = templateCompiler;
            _connector = connector;
            _notificationTemplateCache = new ConcurrentDictionary<string, Func<object, string>>();
        }

        public Task<bool> IsSupportedAsync()
        {
            return Task.FromResult(_connector != null);
        }

        public async Task SendNotificationAsync(INotification notification)
        {
            var template = _notificationTemplateCache.GetOrAdd(notification.TemplateKey, p => _templateCompiler.Compile($"{{{{> {p}}}}}"));

            object data = notification;

            if (notification is IViewData viewData)
            {
                var target = new Dictionary<string, object>();
                viewData.CopyTo(target);
                data = viewData;
            }

            var content = template(data);

            await _connector.SendAsync(notification.Target, notification.Subject, content).ConfigureAwait(false);
        }
    }
}
