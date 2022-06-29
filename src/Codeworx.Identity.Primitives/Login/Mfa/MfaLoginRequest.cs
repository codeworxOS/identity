using System.Security.Claims;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaLoginRequest
    {
        public MfaLoginRequest(ClaimsIdentity identity, string returnUrl = null)
        {
            Identity = identity;
            ReturnUrl = returnUrl;
        }

        public ClaimsIdentity Identity { get; }

        public string ReturnUrl { get; }
    }
}
