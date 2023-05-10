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
        private readonly IRequestEntityCache _cache;

        public EntityTenantService(TContext context, IRequestEntityCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<TenantInfo>> GetTenantsAsync()
        {
            return await _cache.GetTenantInfos(async () =>
            {
                var tenantSet = _context.Set<Tenant>();

                var tenants = await tenantSet
                        .Select(p => new TenantInfo
                        {
                            Key = p.Id.ToString("N"),
                            Name = p.Name,
                            AuthenticationMode = p.AuthenticationMode,
                        })
                        .ToListAsync();

                return tenants;
            });
        }

        public async Task<IEnumerable<TenantInfo>> GetTenantsByIdentityAsync(IIdentityDataParameters request)
        {
            return await _cache.GetTenantInfos(Guid.Parse(request.IdentityUser.Identity), GetTenantInfo);
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
                                    AuthenticationMode = p.Tenant.AuthenticationMode,
                                })
                                .ToListAsync();

            return tenants;
        }
    }
}