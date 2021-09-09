using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Mail
{
    public class SmtpMailConnector : IMailConnector, IDisposable
    {
        private readonly IDisposable _subscription;
        private bool _disposedValue;
        private SmtpOptions _options;

        public SmtpMailConnector(IOptionsMonitor<SmtpOptions> options)
        {
            _subscription = options.OnChange(p => _options = p);
            _options = options.CurrentValue;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task SendAsync(IUser recipient, string subject, string content)
        {
            var client = new SmtpClient(_options.Host, _options.Port);

            if (_options.UserName != null)
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential
                {
                    UserName = _options.UserName,
                    Password = _options.Password,
                };
            }

            client.TargetName = _options.TargetName;
            client.EnableSsl = true;

            var message = new MailMessage(_options.Sender, recipient.Name);
            message.Body = content;
            message.IsBodyHtml = true;
            message.Subject = subject;

            await client.SendMailAsync(message).ConfigureAwait(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _subscription.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
