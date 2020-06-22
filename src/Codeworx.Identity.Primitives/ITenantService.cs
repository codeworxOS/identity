using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface ITenantService
    {
        Task<IEnumerable<TenantInfo>> GetTenantsByIdentityAsync(IIdentityDataParameters request);

        Task<IEnumerable<TenantInfo>> GetTenantsAsync();
    }
}