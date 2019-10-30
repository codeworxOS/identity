namespace Codeworx.Identity.Model
{
    public class SignInResponse
    {
        public SignInResponse(IdentityData identity, string returnUrl)
        {
            Identity = identity;
            ReturnUrl = returnUrl;
        }

        public IdentityData Identity { get; }

        public string ReturnUrl { get; }
    }
}