using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Mail
{
    public class SmtpMailConnector : IMailConnector, IMailDataConnector
    {
        private readonly SmtpOptions _options;

        public SmtpMailConnector(IOptionsSnapshot<SmtpOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendAsync(MailAddress recipient, string subject, string content)
        {
            await SendAsync(new MailData(recipient, subject, content)).ConfigureAwait(false);
        }

        public async Task SendAsync(MailData mail, CancellationToken token = default)
        {
            using (var client = new SmtpClient(_options.Host, _options.Port))
            {
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
                client.EnableSsl = _options.EnableSsl;

                var to = mail.Recipient;
                var from = new MailAddress(_options.Sender);

                using (var message = new MailMessage(from, to))
                {
                    message.Body = mail.Content;
                    message.IsBodyHtml = true;
                    message.Subject = mail.Subject;

                    foreach (var attachment in mail.Attachments)
                    {
                        message.Attachments.Add(attachment);
                    }

                    using (token.Register(() => client.SendAsyncCancel()))
                    {
                        await client.SendMailAsync(message).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
