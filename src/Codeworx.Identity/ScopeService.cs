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

        public async Task<IEnumerable<IScope>> GetScopes()
        {
            var systemScopeTasks = _systemScopeProviders.Select(p => p.GetScopes()).ToList();
            await Task.WhenAll(systemScopeTasks).ConfigureAwait(false);

            var scopes = systemScopeTasks.SelectMany(p => p.Result).ToList();

            if (_scopeProvider != null)
            {
                scopes.AddRange(await _scopeProvider.GetScopes().ConfigureAwait(false));
            }

            return scopes;
        }
    }
}
