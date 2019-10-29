using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class EntityTenantService<TContext> : ITenantService
        where TContext : DbContext
    {
        private readonly TContext _context;

        public EntityTenantService(TContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<TenantInfo>> GetTenantsByIdentityAsync(ClaimsIdentity identity)
        {
            var data = identity.ToIdentityData();

            return this.GetTenantInfo(Guid.Parse(data.Identifier));
        }

        public Task<IEnumerable<TenantInfo>> GetTenantsByUserAsync(IUser user)
        {
            return this.GetTenantInfo(Guid.Parse(user.Identity));
        }

        private async Task<IEnumerable<TenantInfo>> GetTenantInfo(Guid identifier)
        {
            var tenantSet = _context.Set<TenantUser>();

            var tenants = await tenantSet
                                .Where(p => p.RightHolderId == identifier)
                                .Select(p => new TenantInfo
                                {
                                    Key = p.TenantId.ToString("N"),
                                    Name = p.Tenant.Name
                                })
                                .ToListAsync();

            return tenants;
        }
    }
}