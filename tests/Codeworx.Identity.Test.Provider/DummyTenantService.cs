using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.Test
{
    public class DummyTenantService : ITenantService
    {
        public Task<IEnumerable<TenantInfo>> GetTenantsAsync()
        {
            return GetTenants(Constants.MultiTenantUserId);
        }

        public Task<IEnumerable<TenantInfo>> GetTenantsByIdentityAsync(IIdentityDataParameters request)
        {
            return GetTenants(request.User.GetUserId());
        }

        private Task<IEnumerable<TenantInfo>> GetTenants(string identity)
        {
            IEnumerable<TenantInfo> tenants;

            if (Guid.Parse(identity) == Guid.Parse(Constants.MultiTenantUserId))
            {
                tenants = new[]
                          {
                              new TenantInfo { Key = Constants.DefaultTenantId, Name = Constants.DefaultTenantName },
                              new TenantInfo { Key = Constants.DefaultSecondTenantId, Name = Constants.DefaultSecondTenantName }
                          };
            }
            else
            {
                tenants = new[]
                          {
                              new TenantInfo { Key = Constants.DefaultTenantId, Name = Constants.DefaultTenantName }
                          };
            }

            return Task.FromResult<IEnumerable<TenantInfo>>(tenants);
        }
    }
}