using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface ITenantService
    {
        Task<IEnumerable<TenantInfo>> GetTenantsAsync(IUser user);
    }
}