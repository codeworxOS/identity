namespace Codeworx.Identity.Model
{
    public class LoginRequest
    {
        public LoginRequest(string returnUrl, string prompt, string loginProviderId = null, string loginProviderError = null)
        {
            ReturnUrl = returnUrl;
            Prompt = prompt;
            LoginProviderId = loginProviderId;
            LoginProviderError = loginProviderError;
        }

        public string ReturnUrl { get; }

        public string Prompt { get; }

        public string LoginProviderError { get; }

        public string LoginProviderId { get; }
    }
}