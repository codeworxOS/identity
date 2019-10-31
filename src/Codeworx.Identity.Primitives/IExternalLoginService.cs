using System;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IExternalLoginService
    {
        Task<Type> GetParameterTypeAsync(string providerId);

        Task<ProviderInfosResponse> GetProviderInfosAsync(ProviderRequest request);

        Task<SignInResponse> SignInAsync(string providerId, object parameter);
    }
}