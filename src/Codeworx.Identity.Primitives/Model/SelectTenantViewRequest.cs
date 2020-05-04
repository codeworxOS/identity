using System.Security.Claims;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.Model
{
    public class SelectTenantViewRequest
    {
        public SelectTenantViewRequest(AuthorizationRequest request, ClaimsIdentity identity, string requestPath)
        {
            Request = request;
            Identity = identity;
            RequestPath = requestPath;
        }

        public ClaimsIdentity Identity { get; }

        public string RequestPath { get; }

        public AuthorizationRequest Request { get; }
    }
}