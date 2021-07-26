namespace Codeworx.Identity.Login
{
    internal class FormsProfileRegistrationInfo : ILoginRegistrationInfo
    {
        public FormsProfileRegistrationInfo(string providerId, string userName, bool canChangePassword, string passwordChangeUrl, string error = null)
        {
            UserName = userName;
            CanChangePassword = canChangePassword;
            PasswordChangeUrl = passwordChangeUrl;
            ProviderId = providerId;
            Error = error;
        }

        public string UserName { get; }

        public bool CanChangePassword { get; }

        public string PasswordChangeUrl { get; }

        public string ProviderId { get; }

        public string Error { get; }

        public string Template => Constants.Templates.FormsProfile;

        public bool HasRedirectUri(out string redirectUrl)
        {
            redirectUrl = null;
            return false;
        }
    }
}
