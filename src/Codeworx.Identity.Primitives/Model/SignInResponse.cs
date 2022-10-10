using System.Security.Claims;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Model
{
    public class SignInResponse
    {
        public SignInResponse(ClaimsIdentity identity, string returnUrl, AuthenticationMode mode = AuthenticationMode.Login, bool remember = false)
        {
            ReturnUrl = returnUrl;

            var data = new SignInData(identity, remember);

            if (mode == AuthenticationMode.Login)
            {
                Login = data;
            }
            else
            {
                Mfa = data;
            }
        }

        public SignInResponse(ClaimsIdentity login, ClaimsIdentity mfa, string returnUrl)
        {
            ReturnUrl = returnUrl;

            Login = new SignInData(login, false);
            Mfa = new SignInData(mfa, false);
        }

        public SignInData Login { get; set; }

        public SignInData Mfa { get; set; }

        public string ReturnUrl { get; }

        public class SignInData
        {
            public SignInData(ClaimsIdentity identity, bool remember)
            {
                Identity = identity;
                Remember = remember;
            }

            public ClaimsIdentity Identity { get; }

            public bool Remember { get; }
        }
    }
}