using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public class SystemScopeProvider : ISystemScopeProvider
    {
        public Task<IEnumerable<IScope>> GetScopes()
        {
            return Task.FromResult<IEnumerable<IScope>>(new[]
            {
                new Scope(Constants.OpenId.Scopes.OpenId),
                new Scope(Constants.OpenId.Scopes.Profile),
                new Scope(Constants.OpenId.Scopes.OfflineAccess),
            });
        }

        private class Scope : IScope
        {
            public Scope(string key)
            {
                ScopeKey = key;
            }

            public string ScopeKey { get; }
        }
    }
}
