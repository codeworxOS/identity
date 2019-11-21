using System.Security.Claims;

namespace Codeworx.Identity.Model
{
    public class TenantMissingRequest : LoginRequest
    {
        public TenantMissingRequest(string returnUrl, ClaimsIdentity identity)
            : base(returnUrl)
        {
            Identity = identity;
        }

        public ClaimsIdentity Identity { get; }
    }
}