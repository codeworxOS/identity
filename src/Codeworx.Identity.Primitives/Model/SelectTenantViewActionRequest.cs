using System.Security.Claims;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.Model
{
    public class SelectTenantViewActionRequest : SelectTenantViewRequest
    {
        public SelectTenantViewActionRequest(AuthorizationRequest request, ClaimsIdentity identity, string requestPath, string tenantKey, bool setDefault)
            : base(request, identity, requestPath)
        {
            SetDefault = setDefault;
            TenantKey = tenantKey;
        }

        public bool SetDefault { get; }

        public string TenantKey { get; }
    }
}