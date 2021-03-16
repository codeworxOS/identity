namespace Codeworx.Identity.Login
{
    public class LoginRedirectResponse
    {
        public LoginRedirectResponse(string providerId = null, string providerError = null, string redirectUri = null)
        {
            ProviderError = providerError;
            RedirectUri = redirectUri;
            ProviderId = providerId;
        }

        public string ProviderError { get; }

        public string ProviderId { get; }

        public string RedirectUri { get; }
    }
}
