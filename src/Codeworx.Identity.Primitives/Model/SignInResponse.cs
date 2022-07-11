using System.Security.Claims;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Model
{
    public class SignInResponse
    {
        public SignInResponse(ClaimsIdentity identity, string returnUrl, AuthenticationMode mode = AuthenticationMode.Login, bool remember = false)
        {
            Identity = identity;
            ReturnUrl = returnUrl;
            Remember = remember;
            Mode = mode;
        }

        public ClaimsIdentity Identity { get; }

        public AuthenticationMode Mode { get; }

        public bool Remember { get; }

        public string ReturnUrl { get; }
    }
}