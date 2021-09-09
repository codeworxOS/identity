using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Mail
{
    public interface IMailConnector
    {
        Task SendAsync(IUser recipient, string subject, string content);
    }
}
