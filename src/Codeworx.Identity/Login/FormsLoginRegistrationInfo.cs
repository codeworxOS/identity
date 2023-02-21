using Codeworx.Identity.Configuration;

namespace Codeworx.Identity.Login
{
    internal class FormsLoginRegistrationInfo : ILoginRegistrationInfo
    {
        public FormsLoginRegistrationInfo(string providerId, string userName, MaxLengthOption maxLength, string error = null, string forgotPasswordUrl = null, bool showRememberMe = false)
        {
            UserName = userName;
            MaxLength = maxLength;
            ProviderId = providerId;
            Error = error;
            ForgotPasswordUrl = forgotPasswordUrl;
            ShowRememberMe = showRememberMe;
        }

        public string UserName { get; }

        public string ProviderId { get; }

        public string Error { get; }

        public string ForgotPasswordUrl { get; }

        public bool ShowRememberMe { get; }

        public MaxLengthOption MaxLength { get; }

        public string Template => Constants.Templates.FormsLogin;

        public bool HasRedirectUri(out string redirectUrl)
        {
            redirectUrl = null;
            return false;
        }
    }
}