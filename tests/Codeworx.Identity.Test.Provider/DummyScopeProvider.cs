using System.Collections.Generic;

namespace Codeworx.Identity.Test.Provider
{
    using System.Threading.Tasks;
    using Codeworx.Identity.Model;

    public class DummyScopeProvider : IScopeProvider
    {
        public Task<IEnumerable<IScope>> GetScopes()
        {
            IEnumerable<IScope> scopes = new[] {
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

            return Task.FromResult(scopes);
        }
    }
}
