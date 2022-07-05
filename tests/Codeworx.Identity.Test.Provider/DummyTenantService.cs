using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Test.Provider;

namespace Codeworx.Identity.Test
{
    public class DummyTenantService : ITenantService
    {
        public Task<IEnumerable<TenantInfo>> GetTenantsAsync()
        {
            return GetTenants(TestConstants.Users.MultiTenant.UserId);
        }

        public Task<IEnumerable<TenantInfo>> GetTenantsByIdentityAsync(IIdentityDataParameters request)
        {
            return GetTenants(request.User.GetUserId());
        }

        private Task<IEnumerable<TenantInfo>> GetTenants(string identity)
        {
            IEnumerable<TenantInfo> tenants;

            if (Guid.Parse(identity) == Guid.Parse(TestConstants.Users.MultiTenant.UserId))
            {
                tenants = new[]
                          {
                              new TenantInfo { Key = TestConstants.Tenants.DefaultTenant.Id, Name = TestConstants.Tenants.DefaultTenant.Name },
                              new TenantInfo { Key = TestConstants.Tenants.DefaultSecondTenant.Id, Name = TestConstants.Tenants.DefaultSecondTenant.Name }
                          };
            }
            if (Guid.Parse(identity) == Guid.Parse(TestConstants.Users.MfaRequiredOnTenant.UserId))
            {
                tenants = new[]
                          {
                              new TenantInfo { Key = TestConstants.Tenants.MfaTenant.Id, Name = TestConstants.Tenants.MfaTenant.Name, AuthenticationMode = AuthenticationMode.Mfa },
                          };
            }
            else
            {
                tenants = new[]
                          {
                              new TenantInfo { Key = TestConstants.Tenants.DefaultTenant.Id, Name = TestConstants.Tenants.DefaultTenant.Name }
                          };
            }

            return Task.FromResult<IEnumerable<TenantInfo>>(tenants);
        }
    }
}