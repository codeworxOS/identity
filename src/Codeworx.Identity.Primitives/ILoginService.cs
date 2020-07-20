using System;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface ILoginService
    {
        Task<Type> GetParameterTypeAsync(string providerId);

        Task<RegistrationInfoResponse> GetRegistrationInfosAsync(ProviderRequest request);

        Task<SignInResponse> SignInAsync(ILoginRequest parameter);
    }
}