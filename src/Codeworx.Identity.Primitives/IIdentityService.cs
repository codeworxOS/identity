using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface IIdentityService
    {
        Task<IdentityData> GetIdentityAsync(string identity, string tenantKey);

        Task<IdentityData> LoginAsync(string username, string password);

        Task<IdentityData> LoginExternalAsync(string provider, string nameIdentifier);
    }
}