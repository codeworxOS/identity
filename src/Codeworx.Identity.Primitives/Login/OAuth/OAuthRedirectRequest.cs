namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthRedirectRequest
    {
        public OAuthRedirectRequest(string providerId, string returnUrl)
        {
            ProviderId = providerId;
            ReturnUrl = returnUrl;
        }

        public string ProviderId { get; }

        public string ReturnUrl { get; }
    }
}
