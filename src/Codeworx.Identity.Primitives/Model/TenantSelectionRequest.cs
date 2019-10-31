using System.Security.Claims;

namespace Codeworx.Identity.Model
{
    public class TenantSelectionRequest : TenantMissingRequest
    {
        public TenantSelectionRequest(string returnUrl, ClaimsIdentity identity, string tenantKey, bool setDefault)
            : base(returnUrl, identity)
        {
            SetDefault = setDefault;
            TenantKey = tenantKey;
        }

        public bool SetDefault { get; }

        public string TenantKey { get; }
    }
}