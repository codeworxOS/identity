using System;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Mfa.Mail
{
    public class MailMfaLoginProcessor : ILoginProcessor
    {
        private readonly IUserService _userService;

        public MailMfaLoginProcessor(IUserService userService)
        {
            _userService = userService;
        }

        public Type RequestParameterType { get; } = typeof(MailLoginRequest);

        public async Task<ILoginRegistrationInfo> GetRegistrationInfoAsync(ProviderRequest request, ILoginRegistration registration)
        {
            string error = null;
            request.ProviderErrors.TryGetValue(registration.Id, out error);


            switch (request.Type)
            {
                case ProviderRequestType.MfaLogin:
                    var email = await _userService.GetProviderValueAsync(request.User.Identity, registration.Id);
                    if (email == null)
                    {
                        return null;
                    }

                    return new MailRegistrationInfo(registration.Id, email, error);
                case ProviderRequestType.MfaRegister:
                    return new RegisterMailRegistrationInfo(registration.Id, error);
                case ProviderRequestType.Login:
                case ProviderRequestType.Invitation:
                case ProviderRequestType.Profile:
                default:
                    return null;
            }
        }

        public Task<SignInResponse> ProcessAsync(ILoginRegistration configuration, object request)
        {
            throw new NotImplementedException();
        }
    }
}
