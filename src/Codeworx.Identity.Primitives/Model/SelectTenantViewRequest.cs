using System.Security.Claims;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.Model
{
    public class SelectTenantViewRequest
    {
        public SelectTenantViewRequest(AuthorizationRequest request, ClaimsIdentity identity)
        {
            Request = request;
            Identity = identity;
        }

        public ClaimsIdentity Identity { get; }

        public AuthorizationRequest Request { get; }
    }
}