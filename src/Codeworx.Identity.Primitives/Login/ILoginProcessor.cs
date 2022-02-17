using System;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Login
{
    public interface ILoginProcessor
    {
        Type RequestParameterType { get; }

        Task<ILoginRegistrationInfo> GetRegistrationInfoAsync(ProviderRequest request, ILoginRegistration registration);

        Task<SignInResponse> ProcessAsync(ILoginRegistration configuration, object request);

        Task<string> GetReturnUrl(ILoginRegistration registration, object request);
    }
}