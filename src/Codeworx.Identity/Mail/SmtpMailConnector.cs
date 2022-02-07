using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Mail
{
    public class SmtpMailConnector : IMailConnector
    {
        private readonly IMailAddressProvider _mailAddressProvider;
        private readonly SmtpOptions _options;

        public SmtpMailConnector(IOptionsSnapshot<SmtpOptions> options, IMailAddressProvider mailAddressProvider)
        {
            _options = options.Value;
            _mailAddressProvider = mailAddressProvider;
        }

        public async Task SendAsync(IUser recipient, string subject, string content)
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
                client.EnableSsl = true;

                var recipientMail = await _mailAddressProvider.GetMailAdressAsync(recipient);
                using (var message = new MailMessage(_options.Sender, recipientMail))
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
