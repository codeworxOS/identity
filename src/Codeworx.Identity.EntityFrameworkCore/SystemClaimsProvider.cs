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

        public SystemClaimsProvider(TContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<AssignedClaim>> GetClaimsAsync(IIdentityDataParameters parameters)
        {
            var result = new List<AssignedClaim>();

            if (parameters.Scopes.Contains(Constants.Scopes.Groups) || parameters.Scopes.Contains(Constants.Scopes.GroupNames))
            {
                var search = new List<Guid>();
                search.Add(Guid.Parse(parameters.User.GetUserId()));

                var found = new HashSet<Guid>();

                while (search.Any())
                {
                    var nextLayer = await _db.Set<RightHolder>()
                                        .Where(p => search.Contains(p.Id))
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
                                result.Add(new AssignedClaim(new[] { Constants.Claims.GroupNames, item.GroupId.ToString("N") }, new[] { item.Name }, ClaimTarget.AllTokens));
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
