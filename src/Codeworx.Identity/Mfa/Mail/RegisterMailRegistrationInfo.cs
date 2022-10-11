using System;
using System.Collections.Generic;
using System.Text;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Mfa.Mail
{
    public class RegisterMailRegistrationInfo : ILoginRegistrationInfo
    {
        public string Template => Constants.Templates.MailMfaRegistration;

        public string Error => throw new NotImplementedException();

        public string ProviderId => throw new NotImplementedException();

        public bool HasRedirectUri(out string redirectUri)
        {
            throw new NotImplementedException();
        }
    }
}
