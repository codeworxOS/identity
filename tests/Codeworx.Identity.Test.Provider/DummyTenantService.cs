using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.Test
{
    public class DummyTenantService : ITenantService
    {
        public Task<IEnumerable<TenantInfo>> GetTenantsAsync()
        {
            return GetTenants(Constants.TestData.Users.MultiTenant.UserId);
        }

        public Task<IEnumerable<TenantInfo>> GetTenantsByIdentityAsync(IIdentityDataParameters request)
        {
            return GetTenants(request.User.GetUserId());
        }

        private Task<IEnumerable<TenantInfo>> GetTenants(string identity)
        {
            IEnumerable<TenantInfo> tenants;

            if (Guid.Parse(identity) == Guid.Parse(Constants.TestData.Users.MultiTenant.UserId))
            {
                tenants = new[]
                          {
                              new TenantInfo { Key = Constants.TestData.Tenants.DefaultTenant.Id, Name = Constants.TestData.Tenants.DefaultTenant.Name },
                              new TenantInfo { Key = Constants.TestData.Tenants.DefaultSecondTenant.Id, Name = Constants.TestData.Tenants.DefaultSecondTenant.Name }
                          };
            }
            else
            {
                tenants = new[]
                          {
                              new TenantInfo { Key = Constants.TestData.Tenants.DefaultTenant.Id, Name = Constants.TestData.Tenants.DefaultTenant.Name }
                          };
            }

            return Task.FromResult<IEnumerable<TenantInfo>>(tenants);
        }
    }
}