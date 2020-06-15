using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IIdentityService
    {
        Task<IdentityData> GetIdentityAsync(IIdentityDataParameters identityDataParameters);

        Task<ClaimsIdentity> LoginAsync(string username, string password);

        Task<ClaimsIdentity> LoginExternalAsync(string provider, string nameIdentifier);

        Task<ClaimsIdentity> GetClaimsIdentityFromUserAsync(IUser user);
    }
}