using System.Security.Claims;

namespace Codeworx.Identity.Model
{
    public class PasswordChangeRequest
    {
        public PasswordChangeRequest(ClaimsIdentity identity, string returnUrl = null, string prompt = null)
        {
            Identity = identity;
            ReturnUrl = returnUrl;
            Prompt = prompt;
        }

        public ClaimsIdentity Identity { get; }

        public string ReturnUrl { get; }

        public string Prompt { get; }
    }
}
