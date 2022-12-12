using System.Security.Claims;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaLoginRequest
    {
        public MfaLoginRequest(ClaimsIdentity identity, string providerId, string returnUrl = null)
        {
            Identity = identity;
            ProviderId = providerId;
            ReturnUrl = returnUrl;
        }

        public ClaimsIdentity Identity { get; }

        public string ProviderId { get; }

        public string ReturnUrl { get; }
    }
}
