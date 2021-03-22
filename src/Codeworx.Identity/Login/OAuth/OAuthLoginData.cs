using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthLoginData : IExternalLoginData
    {
        private readonly OAuthLoginConfiguration _configuration;

        public OAuthLoginData(ILoginRegistration registration, ClaimsIdentity identity, OAuthLoginConfiguration configuration, string invitationCode)
        {
            _configuration = configuration;
            LoginRegistration = registration;
            Identity = identity;
            InvitationCode = invitationCode;
        }

        public ILoginRegistration LoginRegistration { get; }

        public ClaimsIdentity Identity { get; }

        public string InvitationCode { get; }

        public Task<string> GetExternalIdentifierAsync()
        {
            var claim = Identity.FindFirst(_configuration.IdentifierClaim);
            return Task.FromResult(claim?.Value);
        }
    }
}
