using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IIdentityService
    {
        Task<IdentityData> GetIdentityAsync(IIdentityDataParameters identityDataParameters);

        Task<ClaimsIdentity> LoginAsync(string username, string password);

        Task<ClaimsIdentity> LoginExternalAsync(IExternalLoginData externalLoginData);

        Task<ClaimsIdentity> GetClaimsIdentityFromUserAsync(IUser user);
    }
}