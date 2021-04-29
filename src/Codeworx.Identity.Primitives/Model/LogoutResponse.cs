namespace Codeworx.Identity.Model
{
    public class LogoutResponse
    {
        public LogoutResponse(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; }
    }
}