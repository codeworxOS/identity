using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class EntityClaimsProvider<TContext> : IClaimsProvider
        where TContext : DbContext
    {
        private readonly TContext _db;
        private readonly ITenantService _tenantService;

        public EntityClaimsProvider(TContext db, ITenantService tenantService = null)
        {
            _db = db;
            _tenantService = tenantService;
        }

        public async Task<IEnumerable<AssignedClaim>> GetClaimsAsync(IIdentityDataParameters parameters)
        {
            var assigned = new List<AssignedClaim>();

            string currentTenant = null;
            if (_tenantService != null)
            {
                var tenants = await _tenantService.GetTenantsAsync();
                currentTenant = parameters.Scopes.FirstOrDefault(p => tenants.Any(x => x.Key == p));
            }

            var userId = Guid.Parse(parameters.IdentityUser.Identity);

            IQueryable<ClaimValue> query = _db.Set<ClaimValue>();

            var scopes = await _db.Set<Scope>()
                         .Select(p => new
                         {
                             Id = p.Id,
                             Key = p.Parent != null ? p.Parent.Parent.ScopeKey + ":" + p.ScopeKey : p.ScopeKey,
                         })
                         .Where(p => parameters.Scopes.Contains(p.Key))
                         .ToListAsync();

            if (currentTenant != null)
            {
                var tenantId = Guid.Parse(currentTenant);
                query = query.Where(p =>
                                (p.UserId == userId && (p.TenantId == null || p.TenantId == tenantId)) ||
                                (p.UserId == null && p.TenantId == tenantId) ||
                                (p.UserId == null && p.TenantId == null));
            }
            else
            {
                query = query.Where(p => (p.UserId == userId || p.UserId == null) && p.TenantId == null);
            }

            var scopeIds = scopes.Select(p => p.Id).ToList();

            if (scopeIds.Any())
            {
                query = query
                            .Where(p => p.ClaimType.ScopeClaims.Any(p => scopeIds.Contains(p.ScopeId)));

                var result = await query.Select(p => new
                {
                    TypeKey = p.ClaimType.TypeKey,
                    TypeTarget = p.ClaimType.Target,
                    Value = p.Value,
                })
                            .ToListAsync();

                foreach (var item in result.GroupBy(p => p.TypeKey))
                {
                    var target = item.First().TypeTarget;
                    var key = item.Key;
                    var values = item.Select(p => p.Value).ToList();

                    assigned.Add(AssignedClaim.Create(key, values, target));
                }
            }

            return assigned;
        }
    }
}
