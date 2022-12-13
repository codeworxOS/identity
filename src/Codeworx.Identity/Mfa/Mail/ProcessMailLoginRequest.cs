using System.Security.Claims;

namespace Codeworx.Identity.Mfa.Mail
{
    public class ProcessMailLoginRequest : MailLoginRequest
    {
        public ProcessMailLoginRequest(string providerId, ClaimsIdentity identity, string returnUrl, string oneTimeCode, string sessionId)
            : base(providerId, identity, returnUrl)
        {
            OneTimeCode = oneTimeCode;
            SessionId = sessionId;
        }

        public string OneTimeCode { get; }

        public string SessionId { get; }
    }
}