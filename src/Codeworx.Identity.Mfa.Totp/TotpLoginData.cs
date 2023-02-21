using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Mfa.Totp
{
    public class TotpLoginData : IExternalLoginData
    {
        private readonly string _sharedSecret;

        public TotpLoginData(ILoginRegistration registration, string sharedSecret)
        {
            LoginRegistration = registration;
            _sharedSecret = sharedSecret;
        }

        public ClaimsIdentity Identity => null;

        public string InvitationCode => null;

        public ILoginRegistration LoginRegistration { get; }

        public Task<string> GetExternalIdentifierAsync()
        {
            return Task.FromResult(_sharedSecret);
        }
    }
}