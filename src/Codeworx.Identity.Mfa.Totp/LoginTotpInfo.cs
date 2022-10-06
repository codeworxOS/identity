using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Mfa.Totp
{
    internal class LoginTotpInfo : ILoginRegistrationInfo
    {
        private readonly IUser _user;

        public LoginTotpInfo(IUser user, string providerId, string error = null)
        {
            _user = user;
            ProviderId = providerId;
            Error = error;
        }

        public string Template => TotpConstants.Templates.LoginTotp;

        public string Error { get; }

        public string ProviderId { get; }

        public bool HasRedirectUri(out string redirectUri)
        {
            redirectUri = null;
            return false;
        }
    }
}