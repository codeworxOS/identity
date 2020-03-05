using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth
{
    public class AuthenticateClientResult
    {
        public IClientRegistration ClientRegistration { get; set; }

        public ITokenResult TokenResult { get; set; }
    }
}