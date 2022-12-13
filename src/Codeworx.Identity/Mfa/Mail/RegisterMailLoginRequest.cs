using System.Security.Claims;

namespace Codeworx.Identity.Mfa.Mail
{
    public class RegisterMailLoginRequest : MailLoginRequest
    {
        public RegisterMailLoginRequest(string providerId, ClaimsIdentity identity, string returnUrl, string emailAddress)
            : base(providerId, identity, returnUrl)
        {
            EmailAddress = emailAddress;
        }

        public string EmailAddress { get; }
    }
}