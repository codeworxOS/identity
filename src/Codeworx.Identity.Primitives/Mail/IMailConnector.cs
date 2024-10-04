using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Codeworx.Identity.Mail
{
    public interface IMailConnector
    {
        [Obsolete("Will be removed in future versions. Use SendAsync(MailData, CancellationToken) instead.", false)]
        Task SendAsync(MailAddress recipient, string subject, string content);
    }
}
