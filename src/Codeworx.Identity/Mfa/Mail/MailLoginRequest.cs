using System.Security.Claims;

namespace Codeworx.Identity.Mfa.Mail
{
    public abstract class MailLoginRequest
    {
        public MailLoginRequest(string providerId, ClaimsIdentity identity, string returnUrl, bool rememberMe)
        {
            ProviderId = providerId;
            Identity = identity;
            ReturnUrl = returnUrl;
            RememberMe = rememberMe;
        }

        public string ProviderId { get; }

        public ClaimsIdentity Identity { get; }

        public string ReturnUrl { get; }

        public bool RememberMe { get; }
    }
}