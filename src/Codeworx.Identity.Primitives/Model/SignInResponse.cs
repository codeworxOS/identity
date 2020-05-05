using System.Security.Claims;

namespace Codeworx.Identity.Model
{
    public class SignInResponse
    {
        public SignInResponse(ClaimsIdentity identity, string returnUrl)
        {
            Identity = identity;
            ReturnUrl = returnUrl;
        }

        public ClaimsIdentity Identity { get; }

        public string ReturnUrl { get; }
    }
}