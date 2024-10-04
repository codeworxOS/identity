using System.Collections.Generic;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Notification
{
    public interface IHasAttachments
    {
        Task<IEnumerable<Attachment>> GetAttachmentsAsync(CancellationToken token);
    }
}
