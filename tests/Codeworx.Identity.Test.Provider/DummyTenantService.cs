using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Test.Provider;

namespace Codeworx.Identity.Test
{
    public class DummyTenantService : ITenantService
    {
        private readonly TenantInfo[] _tenants = new[]
                          {
                              new TenantInfo { Key = TestConstants.Tenants.MfaTenant.Id, Name = TestConstants.Tenants.MfaTenant.Name, AuthenticationMode = AuthenticationMode.Mfa },
                              new TenantInfo { Key = TestConstants.Tenants.MfaSecondTenant.Id, Name = TestConstants.Tenants.MfaSecondTenant.Name, AuthenticationMode = AuthenticationMode.Mfa },
                              new TenantInfo { Key = TestConstants.Tenants.DefaultTenant.Id, Name = TestConstants.Tenants.DefaultTenant.Name },
                              new TenantInfo { Key = TestConstants.Tenants.DefaultSecondTenant.Id, Name = TestConstants.Tenants.DefaultSecondTenant.Name }
                          };

        public Task<IEnumerable<TenantInfo>> GetTenantsAsync()
        {
            var tenants = _tenants.ToList();
            return Task.FromResult<IEnumerable<TenantInfo>>(tenants);
        }

        public Task<IEnumerable<TenantInfo>> GetTenantsByIdentityAsync(IIdentityDataParameters request)
        {
            var tenants = GetTenants(request.IdentityUser.Identity);
            return Task.FromResult(tenants);
        }

        private IEnumerable<TenantInfo> GetTenants(string identity)
        {
            if (Guid.Parse(identity) == Guid.Parse(TestConstants.Users.MultiTenant.UserId))
            {
                return _tenants.Where(tenant => tenant.AuthenticationMode == AuthenticationMode.Login).ToList();
            }
            else if (Guid.Parse(identity) == Guid.Parse(TestConstants.Users.MfaTestUser.UserId) || Guid.Parse(identity) == Guid.Parse(TestConstants.Users.MfaTestUserWithMfaRequired.UserId))
            {
                return _tenants.ToList();
            }
            else
            {
                return _tenants.Where(tenant => tenant.Key == TestConstants.Tenants.DefaultTenant.Id).ToList();
            }
        }
    }
}