using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IProviderSetup
    {
        Task<IEnumerable<ExternalProvider>> GetProvidersAsync(string userName = null);

        Task<IUser> GetUserIdentity(string providerId, string nameIdentifier);
    }
}