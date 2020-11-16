using System;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Login
{
    public interface ILoginProcessor
    {
        Type RequestParameterType { get; }

        string Template { get; }

        Task<ILoginRegistrationInfo> GetRegistrationInfoAsync(ProviderRequest request, ILoginRegistration registration);

        Task<SignInResponse> ProcessAsync(ILoginRequest request, ILoginRegistration configuration);
    }
}