namespace Codeworx.Identity.Login
{
    internal class FormsLoginRegistrationInfo : ILoginRegistrationInfo
    {
        public FormsLoginRegistrationInfo(string providerId, string userName, string error = null, string forgotPasswordUrl = null)
        {
            UserName = userName;
            ProviderId = providerId;
            Error = error;
            ForgotPasswordUrl = forgotPasswordUrl;
        }

        public string UserName { get; }

        public string ProviderId { get; }

        public string Error { get; }

        public string ForgotPasswordUrl { get; }

        public string Template => Constants.Templates.FormsLogin;

        public bool HasRedirectUri(out string redirectUrl)
        {
            redirectUrl = null;
            return false;
        }
    }
}