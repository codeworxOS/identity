using System.Security.Claims;

namespace Codeworx.Identity.Mfa.Mail
{
    public abstract class MailLoginRequest
    {
        public MailLoginRequest(string providerId, ClaimsIdentity identity, string returnUrl)
        {
            ProviderId = providerId;
            Identity = identity;
            ReturnUrl = returnUrl;
        }

        public string ProviderId { get; }

        public ClaimsIdentity Identity { get; }

        public string ReturnUrl { get; }
    }
}