namespace Codeworx.Identity.Login
{
    internal class FormsInvitationRegistrationInfo : ILoginRegistrationInfo
    {
        public FormsInvitationRegistrationInfo(string providerId, string userName, bool canChangeLogin, bool canChangePassword, string error = null)
        {
            UserName = userName;
            ProviderId = providerId;
            CanChangeLogin = canChangeLogin;
            CanChangePassword = canChangePassword;
            Error = error;
        }

        public bool CanChangeLogin { get; }

        public bool CanChangePassword { get; }

        public bool CanChangeLoginOrPassword => CanChangeLogin || CanChangePassword;

        public string Error { get; }

        public string ProviderId { get; }

        public string Template => Constants.Templates.FormsInvitation;

        public string UserName { get; }

        public bool HasRedirectUri(out string redirectUrl)
        {
            redirectUrl = null;
            return false;
        }
    }
}