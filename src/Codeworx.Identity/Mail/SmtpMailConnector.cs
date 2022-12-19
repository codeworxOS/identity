using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Mail
{
    public class SmtpMailConnector : IMailConnector
    {
        private readonly SmtpOptions _options;

        public SmtpMailConnector(IOptionsSnapshot<SmtpOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendAsync(MailAddress recipient, string subject, string content)
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

                var to = recipient;
                var from = new MailAddress(_options.Sender);

                using (var message = new MailMessage(from, to))
                {
                    message.Body = content;
                    message.IsBodyHtml = true;
                    message.Subject = subject;

                    await client.SendMailAsync(message).ConfigureAwait(false);
                }
            }
        }
    }
}
