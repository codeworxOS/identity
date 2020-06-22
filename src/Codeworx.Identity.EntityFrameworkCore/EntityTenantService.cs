using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
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

        public async Task<IEnumerable<TenantInfo>> GetTenantsAsync()
        {
            var tenantSet = _context.Set<Tenant>();

            var tenants = await tenantSet
                    .Select(p => new TenantInfo
                    {
                        Key = p.Id.ToString("N"),
                        Name = p.Name,
                    })
                    .ToListAsync();

            return tenants;
        }

        public Task<IEnumerable<TenantInfo>> GetTenantsByIdentityAsync(IIdentityDataParameters request)
        {
            return this.GetTenantInfo(Guid.Parse(request.User.GetUserId()));
        }

        private async Task<IEnumerable<TenantInfo>> GetTenantInfo(Guid identifier)
        {
            var tenantSet = _context.Set<TenantUser>();

            var tenants = await tenantSet
                                .Where(p => p.RightHolderId == identifier)
                                .Select(p => new TenantInfo
                                {
                                    Key = p.TenantId.ToString("N"),
                                    Name = p.Tenant.Name,
                                })
                                .ToListAsync();

            return tenants;
        }
    }
}