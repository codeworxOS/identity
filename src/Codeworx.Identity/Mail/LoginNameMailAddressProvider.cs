using System.Net.Mail;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Mail
{
    public class LoginNameMailAddressProvider : IMailAddressProvider
    {
        public Task<MailAddress> GetMailAdressAsync(IUser user)
        {
            return Task.FromResult(new MailAddress(user.Name));
        }
    }
}
