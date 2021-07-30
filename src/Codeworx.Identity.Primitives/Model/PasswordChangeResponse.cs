namespace Codeworx.Identity.Model
{
    public class PasswordChangeResponse
    {
        public PasswordChangeResponse(string returnUrl, string prompt)
        {
            ReturnUrl = returnUrl;
            Prompt = prompt;
        }

        public string ReturnUrl { get; }

        public string Prompt { get; }
    }
}
