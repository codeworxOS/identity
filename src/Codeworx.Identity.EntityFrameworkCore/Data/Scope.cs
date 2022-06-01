using Codeworx.Identity.Model;

namespace Codeworx.Identity.EntityFrameworkCore.Data
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
