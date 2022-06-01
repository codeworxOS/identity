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
        private readonly IMailAddressProvider _mailAddressProvider;
        private readonly ConcurrentDictionary<string, Func<object, string>> _notificationTemplateCache;

        public NotificationService(
            ITemplateCompiler templateCompiler,
            IMailConnector connector = null,
            IMailAddressProvider mailAddressProvider = null)
        {
            _templateCompiler = templateCompiler;
            _connector = connector;
            _mailAddressProvider = mailAddressProvider;
            _notificationTemplateCache = new ConcurrentDictionary<string, Func<object, string>>();
        }

        public Task<bool> IsSupportedAsync()
        {
            return Task.FromResult(_connector != null && _mailAddressProvider != null);
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
            var recipient = await _mailAddressProvider.GetMailAdressAsync(notification.Target).ConfigureAwait(false);

            await _connector.SendAsync(recipient, notification.Subject, content).ConfigureAwait(false);
        }
    }
}
