using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public class ScopeService : IScopeService
    {
        private readonly IEnumerable<ISystemScopeProvider> _systemScopeProviders;
        private readonly IScopeProvider _scopeProvider;

        public ScopeService(IEnumerable<ISystemScopeProvider> systemScopeProviders, IScopeProvider scopeProvider = null)
        {
            _systemScopeProviders = systemScopeProviders;
            _scopeProvider = scopeProvider;
        }

        public async Task<IEnumerable<IScope>> GetScopes(IClientRegistration client = null)
        {
            var scopes = new List<IScope>();
            foreach (var provider in _systemScopeProviders)
            {
                scopes.AddRange(await provider.GetScopes().ConfigureAwait(false));
            }

            if (_scopeProvider != null)
            {
                scopes.AddRange(await _scopeProvider.GetScopes().ConfigureAwait(false));
            }

            if (client.AllowedScopes?.Count > 0)
            {
                var scopeKeys = client.AllowedScopes.Select(p => p.ScopeKey).ToList();
                for (int i = scopes.Count - 1; i >= 0; i--)
                {
                    var toCheck = scopes[i].ScopeKey;
                    if (!scopeKeys.Contains(toCheck))
                    {
                        if (toCheck.Contains(":") && scopeKeys.Contains(toCheck.Split(':')[0]))
                        {
                            continue;
                        }

                        scopes.RemoveAt(i);
                    }
                }
            }

            return scopes;
        }
    }
}
