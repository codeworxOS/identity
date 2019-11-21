using System.Collections.Generic;
using System.Collections.Immutable;

namespace Codeworx.Identity.Model
{
    public class TenantMissingResponse
    {
        public TenantMissingResponse(IEnumerable<TenantInfo> tenants, bool canSetDefault, string returnUrl)
        {
            Tenants = tenants.ToImmutableList();
            CanSetDefault = canSetDefault;
            ReturnUrl = returnUrl;
        }

        public bool CanSetDefault { get; }

        public string ReturnUrl { get; }

        public IEnumerable<TenantInfo> Tenants { get; }
    }
}