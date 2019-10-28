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

        public async Task<IEnumerable<TenantInfo>> GetTenantsByIdentityAsync(ClaimsIdentity identity)
        {
            var data = identity.ToIdentityData();

            var tenantSet = _context.Set<TenantUser>();
            var id = Guid.Parse(data.Identifier);

            var tenants = await tenantSet
                                .Include(p => p.Tenant)
                                .Where(p => p.RightHolderId == id)
                                .ToListAsync();

            var tenantInfos = tenants
                              .Select(p => new TenantInfo
                              {
                                  Key = p.TenantId.ToString("N"),
                                  Name = p.Tenant.Name
                              })
                              .ToList();

            return tenantInfos;
        }

        public async Task<IEnumerable<TenantInfo>> GetTenantsByUserAsync(IUser user)
        {
            var tenantSet = _context.Set<TenantUser>();
            var id = Guid.Parse(user.Identity);

            var tenants = await tenantSet
                                .Include(p => p.Tenant)
                                .Where(p => p.RightHolderId == id)
                                .ToListAsync();

            var tenantInfos = tenants
                              .Select(p => new TenantInfo
                              {
                                  Key = p.TenantId.ToString("N"),
                                  Name = p.Tenant.Name
                              })
                              .ToList();

            return tenantInfos;
        }
    }
}