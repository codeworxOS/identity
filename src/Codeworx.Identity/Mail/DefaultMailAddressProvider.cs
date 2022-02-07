using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Mail
{
    public class DefaultMailAddressProvider : IMailAddressProvider
    {
        public Task<string> GetMailAdressAsync(IUser user)
        {
            return Task.FromResult(user.Name);
        }
    }
}
