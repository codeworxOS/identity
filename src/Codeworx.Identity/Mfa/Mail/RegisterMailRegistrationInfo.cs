using Codeworx.Identity.Login;

namespace Codeworx.Identity.Mfa.Mail
{
    public class RegisterMailRegistrationInfo : ILoginRegistrationInfo
    {
        public RegisterMailRegistrationInfo(string providerId, string error = null)
        {
            ProviderId = providerId;
            Error = error;
        }

        public string Template => Constants.Templates.MailMfaRegistration;

        public string Error { get; }

        public string ProviderId { get; }

        public bool HasRedirectUri(out string redirectUri)
        {
            redirectUri = null;
            return false;
        }
    }
}
