using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IOAuthClientService
    {
        Task<IEnumerable<IOAuthClientRegistration>> GetForTenantByIdentifier(string tenantIdentifier);
    }
}
