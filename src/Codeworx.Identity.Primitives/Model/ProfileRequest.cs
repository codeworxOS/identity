using System.Security.Claims;

namespace Codeworx.Identity.Model
{
    public class ProfileRequest : LoginRequest
    {
        public ProfileRequest(ClaimsIdentity identity, string loginProviderId, string loginProviderError)
            : base(null, null, loginProviderId, loginProviderError)
        {
            Identity = identity;
        }

        public ClaimsIdentity Identity { get; }
    }
}