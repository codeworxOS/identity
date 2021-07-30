using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public class Scope : IScope
    {
        public Scope(string key)
        {
            ScopeKey = key;
        }

        public string ScopeKey { get; }
    }
}
