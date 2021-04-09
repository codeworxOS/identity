using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public class ExternalTokenScopeProvider : ISystemScopeProvider
    {
        private readonly IExternalTokenCache _externalTokenCache;

        public ExternalTokenScopeProvider(IExternalTokenCache externalTokenCache = null)
        {
            _externalTokenCache = externalTokenCache;
        }

        public Task<IEnumerable<IScope>> GetScopes()
        {
            if (_externalTokenCache != null)
            {
                return Task.FromResult<IEnumerable<IScope>>(new[]
                {
                    new Scope(Constants.Scopes.ExternalToken.All),
                    new Scope(Constants.Scopes.ExternalToken.IdToken),
                    new Scope(Constants.Scopes.ExternalToken.AccessToken),
                });
            }

            return Task.FromResult(Enumerable.Empty<IScope>());
        }
    }
}
