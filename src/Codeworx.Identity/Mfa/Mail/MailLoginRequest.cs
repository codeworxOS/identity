using System.Security.Claims;

namespace Codeworx.Identity.Mfa.Mail
{
    public abstract class MailLoginRequest
    {
        public MailLoginRequest(string providerId, ClaimsIdentity identity, string returnUrl, bool rememberMe, bool noNav)
        {
            ProviderId = providerId;
            Identity = identity;
            ReturnUrl = returnUrl;
            RememberMe = rememberMe;
            NoNav = noNav;
        }

        public string ProviderId { get; }

        public ClaimsIdentity Identity { get; }

        public string ReturnUrl { get; }

        public bool RememberMe { get; }

        public bool NoNav { get; private set; }
    }
}