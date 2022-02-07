using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Mail
{
    public interface IMailAddressProvider
    {
        Task<string> GetMailAdressAsync(IUser user);
    }
}
