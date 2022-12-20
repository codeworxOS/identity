using System;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface ILoginService
    {
        Task<Type> GetParameterTypeAsync(string providerId);

        Task<ILoginRegistration> GetLoginRegistrationInfoAsync(string providerId, LoginProviderType providerType);

        Task<RegistrationInfoResponse> GetRegistrationInfosAsync(ProviderRequest request);

        Task<SignInResponse> SignInAsync(string providerId, object parameter);
    }
}