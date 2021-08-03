namespace Codeworx.Identity.Model
{
    public class LoggedinResponse
    {
        public LoggedinResponse(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; }
    }
}