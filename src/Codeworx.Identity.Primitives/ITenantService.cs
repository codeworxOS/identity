using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface ITenantService
    {
        Task<IEnumerable<TenantInfo>> GetTenantsByIdentityAsync(ClaimsIdentity user);

        Task<IEnumerable<TenantInfo>> GetTenantsByUserAsync(IUser user);

        Task<IEnumerable<TenantInfo>> GetTenantsAsync();
    }
}