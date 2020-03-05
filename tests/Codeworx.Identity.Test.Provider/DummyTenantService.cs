using Codeworx.Identity.Model;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.Test
{
    public class DummyTenantService : ITenantService
    {
        public Task<IEnumerable<TenantInfo>> GetTenantsByIdentityAsync(ClaimsIdentity user)
        {
            var identity = user.ToIdentityData();
            return GetTenants(identity.Identifier);
        }

        public Task<IEnumerable<TenantInfo>> GetTenantsByUserAsync(IUser user)
        {
            return GetTenants(user.Identity);
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