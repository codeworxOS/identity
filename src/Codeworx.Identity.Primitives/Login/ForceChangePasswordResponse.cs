namespace Codeworx.Identity.Login
{
    public class ForceChangePasswordResponse
    {
        public ForceChangePasswordResponse(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; }
    }
}
