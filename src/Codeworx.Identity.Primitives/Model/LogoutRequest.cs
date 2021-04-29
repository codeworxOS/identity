namespace Codeworx.Identity.Model
{
    public class LogoutRequest
    {
        public LogoutRequest(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; }
    }
}