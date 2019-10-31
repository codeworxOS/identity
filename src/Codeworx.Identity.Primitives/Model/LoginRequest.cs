namespace Codeworx.Identity.Model
{
    public class LoginRequest
    {
        public LoginRequest(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; }
    }
}