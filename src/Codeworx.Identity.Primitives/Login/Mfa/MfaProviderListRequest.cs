using System.Security.Claims;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaProviderListRequest
    {
        public MfaProviderListRequest(ClaimsIdentity identity, string returnUrl)
        {
            Identity = identity;
            ReturnUrl = returnUrl;
        }

        public ClaimsIdentity Identity { get; }

        public string ReturnUrl { get; }
    }
}
