using System.Net.Mail;
using System.Threading.Tasks;

namespace Codeworx.Identity.Mail
{
    public interface IMailConnector
    {
        Task SendAsync(MailAddress recipient, string subject, string content);
    }
}
