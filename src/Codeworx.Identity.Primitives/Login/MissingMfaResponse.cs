using System.Security.Claims;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.Login
{
    public class MissingMfaResponse
    {
        public MissingMfaResponse(AuthorizationRequest request, ClaimsIdentity identity)
        {
            Request = request;
            Identity = identity;
        }

        public AuthorizationRequest Request { get; }

        public ClaimsIdentity Identity { get; }
    }
}
