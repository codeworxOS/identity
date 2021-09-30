using Codeworx.Identity.Model;

namespace Codeworx.Identity.Configuration.Model
{
    public class Scope : IScope
    {
        public Scope(string scope)
        {
            ScopeKey = scope;
        }

        public string ScopeKey { get; }
    }
}
