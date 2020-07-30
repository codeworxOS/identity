namespace Codeworx.Identity.Model
{
    public class ProviderRequest
    {
        public ProviderRequest(string returnUrl, string prompt, string userName = null)
        {
            ReturnUrl = returnUrl;
            UserName = userName;
            Prompt = prompt;
        }

        public string ReturnUrl { get; }

        public string UserName { get; }

        public string Prompt { get; }
    }
}