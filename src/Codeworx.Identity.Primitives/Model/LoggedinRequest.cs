using System.Security.Claims;

namespace Codeworx.Identity.Model
{
    public class LoggedinRequest : LoginRequest
    {
        public LoggedinRequest(ClaimsIdentity identity, string returnUrl, bool headerOnly, string loginProviderId, string loginProviderError)
            : base(returnUrl, null, headerOnly, loginProviderId, loginProviderError)
        {
            Identity = identity;
        }

        public ClaimsIdentity Identity { get; }
    }
}