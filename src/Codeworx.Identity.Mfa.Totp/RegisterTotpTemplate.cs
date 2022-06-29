using System.Reflection;

namespace Codeworx.Identity.Mfa.Totp
{
    public class RegisterTotpTemplate : IPartialTemplate
    {
        public RegisterTotpTemplate()
        {
            Template = typeof(RegisterTotpTemplate).Assembly.GetResourceString("Codeworx.Identity.Mfa.Totp.assets.register_totp.html");
        }

        public string Name => TotpConstants.Templates.RegisterTotp;

        public string Template { get; }
    }
}
