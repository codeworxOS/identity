using System.Collections.Generic;

namespace Codeworx.Identity.Test.Provider
{
    using System.Linq;
    using System.Threading.Tasks;
    using Codeworx.Identity.Model;

    public class DummyScopeProvider : IScopeProvider
    {
        private static readonly Scope[] _allScopes;

        static DummyScopeProvider()
        {
            _allScopes = new[] {
                new Scope("scope1"),
                new Scope("scope1:sub1"),
                new Scope("scope1:sub2"),
                new Scope("scope2"),
                new Scope("scope2:sub1"),
                new Scope("scope2:sub2"),
                new Scope("scope3"),
                new Scope("scope3:sub1"),
                new Scope("scope3:sub2"),
                new Scope("scope4"),
                new Scope("scope4:sub1"),
                new Scope("scope4:sub2"),
                new Scope("scope5"),
                new Scope("scope5:sub1"),
                new Scope("scope6:sub2")
            };

        }

        public Task<IEnumerable<IScope>> GetScopes(IIdentityDataParameters parameters = null)
        {
            if (parameters != null)
            {
                var client = ((IDummyClientRegistration)parameters.Client);

                if (client.AllowedScopes.Any())
                {
                    return Task.FromResult<IEnumerable<IScope>>(client.AllowedScopes);
                }
            }

            return Task.FromResult<IEnumerable<IScope>>(_allScopes);
        }
    }
}
