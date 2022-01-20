using System.Security.Claims;

namespace Codeworx.Identity.Model
{
    public class SignInResponse
    {
        public SignInResponse(ClaimsIdentity identity, string returnUrl, bool remember = false)
        {
            Identity = identity;
            ReturnUrl = returnUrl;
            Remember = remember;
        }

        public ClaimsIdentity Identity { get; }

        public string ReturnUrl { get; }

        public bool Remember { get; }
    }
}