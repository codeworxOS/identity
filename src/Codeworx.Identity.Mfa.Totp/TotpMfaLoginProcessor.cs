using System;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Mfa.Totp
{
    public class TotpMfaLoginProcessor : ILoginProcessor
    {
        private readonly IdentityOptions _options;

        public TotpMfaLoginProcessor(IOptionsSnapshot<IdentityOptions> options)
        {
            _options = options.Value;
        }

        public Type RequestParameterType => typeof(TotpLoginRequest);

        public Task<ILoginRegistrationInfo> GetRegistrationInfoAsync(ProviderRequest request, ILoginRegistration registration)
        {
            string error = null;
            request.ProviderErrors.TryGetValue(registration.Id, out error);

            switch (request.Type)
            {
                case ProviderRequestType.MfaLogin:
                    throw new NotSupportedException();
                case ProviderRequestType.MfaRegister:
                    return Task.FromResult<ILoginRegistrationInfo>(new RegisterTotpInfo(request.User, _options, registration.Id, error));
                case ProviderRequestType.Login:
                case ProviderRequestType.Invitation:
                case ProviderRequestType.Profile:
                default:
                    throw new NotSupportedException();
            }
        }

        public Task<SignInResponse> ProcessAsync(ILoginRegistration configuration, object request)
        {
            throw new NotImplementedException();
        }
    }
}
