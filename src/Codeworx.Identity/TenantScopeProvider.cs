using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public class TenantScopeProvider : ISystemScopeProvider
    {
        private readonly ITenantService _tenantService;

        public TenantScopeProvider(ITenantService tenantService = null)
        {
            _tenantService = tenantService;
        }

        public async Task<IEnumerable<IScope>> GetScopes()
        {
            if (_tenantService == null)
            {
                return Enumerable.Empty<IScope>();
            }

            var result = new List<IScope>();
            result.Add(new Scope(Constants.Scopes.Tenant));

            var tenants = await _tenantService.GetTenantsAsync();
            foreach (var tenant in tenants)
            {
                result.Add(new Scope(tenant.Key));
            }

            return result;
        }
    }
}
