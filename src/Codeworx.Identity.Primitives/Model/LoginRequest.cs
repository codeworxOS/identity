namespace Codeworx.Identity.Model
{
    public class LoginRequest
    {
        public LoginRequest(string returnUrl, string prompt, string providerLoginError = null)
        {
            ReturnUrl = returnUrl;
            Prompt = prompt;
            ProviderLoginError = providerLoginError;
        }

        public string ReturnUrl { get; }

        public string Prompt { get; }

        public string ProviderLoginError { get; }
    }
}