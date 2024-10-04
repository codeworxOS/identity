using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Mail
{
    public static class CodeworxIdentityMailConnectorExtensions
    {
        public static async Task SendAsync(this IMailConnector mailConnector, MailData mail, CancellationToken token = default)
        {
            if (mailConnector is IMailDataConnector dataConnector)
            {
                await dataConnector.SendAsync(mail, token).ConfigureAwait(false);
                return;
            }

#pragma warning disable CS0618 // Type or member is obsolete
            await mailConnector.SendAsync(mail.Recipient, mail.Subject, mail.Content).ConfigureAwait(false);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}