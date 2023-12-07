using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class SystemClaimsProvider<TContext> : ISystemClaimsProvider
        where TContext : DbContext
    {
        private readonly TContext _db;
        private readonly ITenantService _tenantService;

        public SystemClaimsProvider(TContext db, ITenantService tenantService = null)
        {
            _db = db;
            _tenantService = tenantService;
        }

        public async Task<IEnumerable<AssignedClaim>> GetClaimsAsync(IIdentityDataParameters parameters)
        {
            var result = new List<AssignedClaim>();

            Guid? tenantIdFilter = null;

            if (parameters.Scopes.Contains(Constants.Scopes.Groups) || parameters.Scopes.Contains(Constants.Scopes.GroupNames))
            {
                if (parameters.Scopes.Contains(Constants.Scopes.Tenant))
                {
                    if (_tenantService != null)
                    {
                        var tenants = await _tenantService.GetTenantsByIdentityAsync(parameters).ConfigureAwait(false);
                        var tenant = tenants.Where(p => parameters.Scopes.Contains(p.Key)).ToList();

                        if (tenant.Count == 1 && Guid.TryParse(tenant[0].Key, out var parsed))
                        {
                            tenantIdFilter = parsed;
                        }
                    }
                }

                var search = new List<Guid>();
                search.Add(Guid.Parse(parameters.IdentityUser.Identity));

                var found = new HashSet<Guid>();

                while (search.Any())
                {
                    var baseQuery = _db.Set<RightHolder>()
                                        .Where(p => search.Contains(p.Id));

                    if (tenantIdFilter.HasValue)
                    {
                        baseQuery = baseQuery.Where(p => p.Tenants.Any(x => x.TenantId == tenantIdFilter.Value));
                    }

                    var nextLayer = await baseQuery
                                        .SelectMany(p => p.MemberOf)
                                        .Select(p => new { p.GroupId, p.Group.Name })
                                        .Distinct()
                                        .ToListAsync()
                                        .ConfigureAwait(false);

                    search.Clear();

                    foreach (var item in nextLayer)
                    {
                        if (!found.Contains(item.GroupId))
                        {
                            found.Add(item.GroupId);
                            search.Add(item.GroupId);

                            if (parameters.Scopes.Contains(Constants.Scopes.GroupNames))
                            {
                                result.Add(new AssignedClaim(new[] { Constants.Claims.GroupNames, item.GroupId.ToString("N") }, new[] { item.Name }, ClaimTarget.All));
                            }
                        }
                    }
                }

                if (parameters.Scopes.Contains(Constants.Scopes.Groups))
                {
                    result.Add(AssignedClaim.Create(Constants.Claims.Group, found.Select(p => p.ToString("N")), ClaimTarget.AccessToken | ClaimTarget.ProfileEndpoint));
                }
            }

            return result;
        }
    }
}
