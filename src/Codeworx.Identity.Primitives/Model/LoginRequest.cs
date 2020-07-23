namespace Codeworx.Identity.Model
{
    public class LoginRequest
    {
        public LoginRequest(string returnUrl, string prompt)
        {
            ReturnUrl = returnUrl;
            Prompt = prompt;
        }

        public string ReturnUrl { get; }

        public string Prompt { get; }
    }
}