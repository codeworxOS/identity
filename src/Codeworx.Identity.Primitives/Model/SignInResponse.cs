namespace Codeworx.Identity.Model
{
    public class SignInResponse
    {
        public SignInResponse(IdentityData identity)
        {
            Identity = identity;
        }

        public IdentityData Identity { get; }
    }
}
