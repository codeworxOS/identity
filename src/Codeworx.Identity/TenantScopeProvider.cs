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

        public async Task<IEnumerable<IScope>> GetScopes(IIdentityDataParameters parameters = null)
        {
            if (_tenantService == null || parameters == null)
            {
                return Enumerable.Empty<IScope>();
            }

            if (parameters.Scopes.Contains(Constants.Scopes.Tenant))
            {
                var result = new List<IScope>();

                result.Add(new Scope(Constants.Scopes.Tenant));

                var tenants = await _tenantService.GetTenantsAsync();
                foreach (var tenant in tenants)
                {
                    result.Add(new Scope(tenant.Key));
                }

                return result;
            }

            return Enumerable.Empty<IScope>();
        }
    }
}
