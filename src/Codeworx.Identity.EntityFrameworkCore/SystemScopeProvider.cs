using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class SystemScopeProvider : ISystemScopeProvider
    {
        public Task<IEnumerable<IScope>> GetScopes(IIdentityDataParameters parameters = null)
        {
            return Task.FromResult<IEnumerable<IScope>>(new[]
                {
                    new DefaultScope(Constants.Scopes.Groups),
                    new DefaultScope(Constants.Scopes.GroupNames),
                });
        }

        private class DefaultScope : IScope
        {
            public DefaultScope(string name)
            {
                ScopeKey = name;
            }

            public string ScopeKey { get; }
        }
    }
}
