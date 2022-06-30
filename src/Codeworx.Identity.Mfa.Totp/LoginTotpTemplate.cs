using System.Reflection;

namespace Codeworx.Identity.Mfa.Totp
{
    public class LoginTotpTemplate : IPartialTemplate
    {
        public LoginTotpTemplate()
        {
            Template = typeof(RegisterTotpTemplate).Assembly.GetResourceString("Codeworx.Identity.Mfa.Totp.assets.login_totp.html");
        }

        public string Name => TotpConstants.Templates.LoginTotp;

        public string Template { get; }
    }
}
