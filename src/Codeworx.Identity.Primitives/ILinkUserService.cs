using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface ILinkUserService
    {
        Task LinkUserAsync(IUser user, IExternalLoginData loginData);

        Task UnlinkUserAsync(IUser user, string providerId);
    }
}
