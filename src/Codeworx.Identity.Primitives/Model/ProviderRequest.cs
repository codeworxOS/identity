namespace Codeworx.Identity.Model
{
    public class ProviderRequest
    {
        public ProviderRequest(string returnUrl, string userName = null)
        {
            ReturnUrl = returnUrl;
            UserName = userName;
        }

        public string ReturnUrl { get; }

        public string UserName { get; }
    }
}