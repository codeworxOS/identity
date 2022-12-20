using System.Security.Claims;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaProviderListRequest
    {
        public MfaProviderListRequest(ClaimsIdentity identity, string returnUrl, bool headerOnly)
        {
            Identity = identity;
            ReturnUrl = returnUrl;
            HeaderOnly = headerOnly;
        }

        public ClaimsIdentity Identity { get; }

        public string ReturnUrl { get; }

        public bool HeaderOnly { get; }
    }
}
