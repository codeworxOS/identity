using System.Net.Mail;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Mail
{
    public interface IMailAddressProvider
    {
        Task<MailAddress> GetMailAdressAsync(IUser user);
    }
}
