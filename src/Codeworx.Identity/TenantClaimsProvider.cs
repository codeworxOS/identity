using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public class TenantClaimsProvider : ISystemClaimsProvider
    {
        private readonly ITenantService _tenantService;

        public TenantClaimsProvider(ITenantService tenantService = null)
        {
            _tenantService = tenantService;
        }

        public async Task<IEnumerable<AssignedClaim>> GetClaimsAsync(IIdentityDataParameters parameters)
        {
            var result = new List<AssignedClaim>();

            if (_tenantService != null)
            {
                var tenants = await _tenantService.GetTenantsByIdentityAsync(parameters).ConfigureAwait(false);
                foreach (var item in tenants)
                {
                    var type = new[] { Constants.Claims.Tenant, item.Key };

                    result.Add(new AssignedClaim(type, new[] { item.Name }, ClaimTarget.AllTokens | ClaimTarget.ProfileEndpoint));
                }

                if (parameters.Scopes.Contains(Constants.Scopes.Tenant))
                {
                    var tenant = tenants.Where(p => parameters.Scopes.Contains(p.Key)).ToList();

                    if (tenant.Count == 1)
                    {
                        result.Add(AssignedClaim.Create(Constants.Claims.CurrentTenant, tenant[0].Key, ClaimTarget.AllTokens | ClaimTarget.ProfileEndpoint));
                    }
                }
            }

            return result;
        }
    }
}
