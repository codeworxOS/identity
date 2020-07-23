namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthRedirectRequest
    {
        public OAuthRedirectRequest(string providerId, string returnUrl, string prompt)
        {
            ProviderId = providerId;
            ReturnUrl = returnUrl;
            Prompt = prompt;
        }

        public string ProviderId { get; }

        public string ReturnUrl { get; }

        public string Prompt { get; }
    }
}
