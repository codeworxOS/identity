using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class EntityScopeProvider<TContext> : IScopeProvider
        where TContext : DbContext
    {
        private readonly TContext _db;

        public EntityScopeProvider(TContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<IScope>> GetScopes(IIdentityDataParameters parameters = null)
        {
            var scopes = Enumerable.Empty<Scope>();

            if (parameters?.Client != null && Guid.TryParse(parameters.Client.ClientId, out var clientId))
            {
                await _db.Set<ScopeAssignment>().Where(p => p.ClientId == clientId)
                    .Select(p => p.Scope)
                    .Include(p => p.Parent.Parent)
                    .AsNoTracking()
                    .ToListAsync();
            }

            if (!scopes.Any())
            {
                scopes = await _db.Set<Scope>()
                            .Include(p => p.Parent.Parent)
                            .AsNoTracking()
                            .ToListAsync();
            }

            var result = new List<IScope>();

            foreach (var item in scopes)
            {
                string key = item.ScopeKey;

                if (item.Parent?.Parent != null)
                {
                    key = $"{item.Parent.Parent.ScopeKey}:{key}";
                }

                result.Add(new Data.Scope(key));
            }

            return result;
        }
    }
}
