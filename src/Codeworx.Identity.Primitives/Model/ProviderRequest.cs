namespace Codeworx.Identity.Model
{
    public class ProviderRequest
    {
        public ProviderRequest(string returnUrl, string userName, string baseUrl)
        {
            ReturnUrl = returnUrl;
            UserName = userName;
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; }

        public string ReturnUrl { get; }

        public string UserName { get; }
    }
}