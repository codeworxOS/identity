using System.Collections.Generic;
using System.Collections.Immutable;

namespace Codeworx.Identity.Model
{
    public class SelectTenantViewResponse
    {
        public SelectTenantViewResponse(IEnumerable<TenantInfo> tenants, bool canSetDefault)
        {
            Tenants = tenants.ToImmutableList();
            CanSetDefault = canSetDefault;
        }

        public bool CanSetDefault { get; }

        public IEnumerable<TenantInfo> Tenants { get; }
    }
}