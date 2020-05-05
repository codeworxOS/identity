using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface IIdentityService
    {
        Task<IdentityData> GetIdentityAsync(IIdentityDataParameters identityDataParameters);

        Task<ClaimsIdentity> LoginAsync(string username, string password);

        Task<ClaimsIdentity> LoginExternalAsync(string provider, string nameIdentifier);
    }
}