using System;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Mfa.Mail
{
    public class MailMfaLoginProcessor : ILoginProcessor
    {
        public Type RequestParameterType { get; } = typeof(MailLoginRequest);

        public Task<ILoginRegistrationInfo> GetRegistrationInfoAsync(ProviderRequest request, ILoginRegistration registration)
        {
            ////switch (request.Type)
            ////{
            ////    case ProviderRequestType.MfaLogin:
            ////        break;
            ////    case ProviderRequestType.MfaRegister:
            ////        break;
            ////}

            return null;
        }

        public Task<SignInResponse> ProcessAsync(ILoginRegistration configuration, object request)
        {
            throw new NotImplementedException();
        }
    }
}
