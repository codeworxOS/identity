using System.Security.Claims;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaLoginRequest
    {
        public MfaLoginRequest(ClaimsIdentity identity, bool headerOnly, string providerId, string returnUrl = null)
        {
            Identity = identity;
            HeaderOnly = headerOnly;
            ProviderId = providerId;
            ReturnUrl = returnUrl;
        }

        public ClaimsIdentity Identity { get; }

        public bool HeaderOnly { get; }

        public string ProviderId { get; }

        public string ReturnUrl { get; }
    }
}
