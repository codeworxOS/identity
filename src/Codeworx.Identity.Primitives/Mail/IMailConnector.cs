using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Mail
{
    public interface IMailConnector
    {
        [Obsolete("Will be removed in future versions. Use SendAsync(recipient,subject,content,attachments, token) instead.", true)]
        Task SendAsync(MailAddress recipient, string subject, string content);

        Task SendAsync(MailAddress recipient, string subject, string content, IEnumerable<Attachment> attachments, CancellationToken token = default);
    }
}
