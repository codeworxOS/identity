namespace Codeworx.Identity.Model
{
    public class ProfileLinkResponse
    {
        public ProfileLinkResponse(string redirectUrl)
        {
            RedirectUrl = redirectUrl;
        }

        public string RedirectUrl { get; }
    }
}