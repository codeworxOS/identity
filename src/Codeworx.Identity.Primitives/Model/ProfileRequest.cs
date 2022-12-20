using System.Security.Claims;

namespace Codeworx.Identity.Model
{
    public class ProfileRequest : LoginRequest
    {
        public ProfileRequest(ClaimsIdentity identity, bool headerOnly, string loginProviderId, string loginProviderError)
            : base(null, null, headerOnly, loginProviderId, loginProviderError)
        {
            Identity = identity;
        }

        public ClaimsIdentity Identity { get; }
    }
}