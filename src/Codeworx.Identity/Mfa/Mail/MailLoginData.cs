using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Mfa.Mail
{
    internal class MailLoginData : IExternalLoginData
    {
        private readonly ILoginRegistration _registration;
        private string _emailAddress;

        public MailLoginData(ILoginRegistration registration, string emailAddress)
        {
            _registration = registration;
            _emailAddress = emailAddress;
        }

        public ILoginRegistration LoginRegistration => _registration;

        public ClaimsIdentity Identity => null;

        public string InvitationCode => null;

        public Task<string> GetExternalIdentifierAsync()
        {
            return Task.FromResult(_emailAddress);
        }
    }
}