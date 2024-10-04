using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Mail
{
    public interface IMailDataConnector
    {
        Task SendAsync(MailData mail, CancellationToken token = default);
    }
}
