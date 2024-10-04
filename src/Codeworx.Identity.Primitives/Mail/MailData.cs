using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Codeworx.Identity.Mail
{
    public class MailData
    {
        public MailData(MailAddress recipient, string subject, string content, IEnumerable<Attachment> attachments = null)
        {
            Recipient = recipient;
            Subject = subject;
            Content = content;
            Attachments = attachments ?? Enumerable.Empty<Attachment>();
        }

        public MailAddress Recipient { get; }

        public string Subject { get; }

        public string Content { get; }

        public IEnumerable<Attachment> Attachments { get; }
    }
}
