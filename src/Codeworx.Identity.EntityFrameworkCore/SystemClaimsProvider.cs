using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Model;
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

        public async Task<IEnumerable<AssignedClaim>> GetClaimsAsync(IUser user, IIdentityDataParameters parameters)
        {
            var result = new List<AssignedClaim>();

            if (parameters.Scopes.Contains(Constants.Scopes.Groups))
            {
                var search = new List<Guid>();
                search.Add(Guid.Parse(user.Identity));

                var found = new HashSet<Guid>();

                while (search.Any())
                {
                    var nextLayer = await _db.Set<RightHolder>()
                                        .Where(p => search.Contains(p.Id))
                                        .SelectMany(p => p.MemberOf)
                                        .Select(p => p.GroupId)
                                        .Distinct()
                                        .ToListAsync()
                                        .ConfigureAwait(false);

                    search.Clear();

                    foreach (var item in nextLayer)
                    {
                        if (!found.Contains(item))
                        {
                            found.Add(item);
                            search.Add(item);
                        }
                    }
                }

                result.Add(AssignedClaim.Create(Constants.Claims.Group, found.Select(p => p.ToString("N")), ClaimTarget.AccessToken));
            }

            return result;
        }
    }
}
