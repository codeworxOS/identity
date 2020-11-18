using System.Collections.Generic;

namespace Codeworx.Identity.Model
{
    public class ProviderRequest
    {
        public ProviderRequest(string returnUrl, string prompt, string userName = null)
        {
            ReturnUrl = returnUrl;
            UserName = userName;
            Prompt = prompt;
            ProviderErrors = new Dictionary<string, string>();
        }

        public Dictionary<string, string> ProviderErrors { get; }

        public string ReturnUrl { get; }

        public string UserName { get; }

        public string Prompt { get; }
    }
}