using System.Security.Claims;
using Codeworx.Identity.Login.Mfa;

namespace Codeworx.Identity.Mfa.Totp
{
    public class TotpLoginRequest
    {
        public TotpLoginRequest(string providerId, ClaimsIdentity identity, MfaAction action, string returnUrl, string oneTimeCode, string sharedSecret = null)
        {
            ProviderId = providerId;
            Identity = identity;
            Action = action;
            ReturnUrl = returnUrl;
            OneTimeCode = oneTimeCode;
            SharedSecret = sharedSecret;
        }

        public string ProviderId { get; }

        public ClaimsIdentity Identity { get; }

        public MfaAction Action { get; }

        public string ReturnUrl { get; }

        public string OneTimeCode { get; }

        public string SharedSecret { get; }
    }
}