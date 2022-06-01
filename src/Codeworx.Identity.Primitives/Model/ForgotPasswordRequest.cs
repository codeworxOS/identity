namespace Codeworx.Identity.Model
{
    public class ForgotPasswordRequest
    {
        public ForgotPasswordRequest(string returnUrl = null, string prompt = null)
        {
            ReturnUrl = returnUrl;
            Prompt = prompt;
        }

        public string ReturnUrl { get; }

        public string Prompt { get; }
    }
}
