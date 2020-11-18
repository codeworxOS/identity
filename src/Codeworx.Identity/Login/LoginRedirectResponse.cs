namespace Codeworx.Identity.Login
{
    public class LoginRedirectResponse
    {
        public LoginRedirectResponse(string providerError, string redirectUri)
        {
            ProviderError = providerError;
            RedirectUri = redirectUri;
        }

        public string ProviderError { get; }

        public string RedirectUri { get; set; }
    }
}
