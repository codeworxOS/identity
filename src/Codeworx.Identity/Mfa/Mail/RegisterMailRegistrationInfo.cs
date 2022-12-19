using Codeworx.Identity.Login;

namespace Codeworx.Identity.Mfa.Mail
{
    public class RegisterMailRegistrationInfo : ILoginRegistrationInfo
    {
        public RegisterMailRegistrationInfo(string providerId, string emailAddress = null, string sessionId = null, string error = null)
        {
            ProviderId = providerId;
            EmailAddress = emailAddress;
            SessionId = sessionId;
            Error = error;
        }

        public string Template => Constants.Templates.RegisterMfaMail;

        public string Error { get; }

        public string ProviderId { get; }

        public string EmailAddress { get; }

        public string SessionId { get; }

        public bool HasRedirectUri(out string redirectUri)
        {
            redirectUri = null;
            return false;
        }
    }
}
