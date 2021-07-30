using System.Collections.Generic;
using System.Collections.Immutable;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class SelectTenantViewResponse : IViewData
    {
        public SelectTenantViewResponse(IEnumerable<TenantInfo> tenants, bool canSetDefault)
        {
            Tenants = tenants.ToImmutableList();
            CanSetDefault = canSetDefault;
        }

        public bool CanSetDefault { get; }

        public IEnumerable<TenantInfo> Tenants { get; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(CanSetDefault), CanSetDefault);
            target.Add(nameof(Tenants), Tenants);
        }
    }
}