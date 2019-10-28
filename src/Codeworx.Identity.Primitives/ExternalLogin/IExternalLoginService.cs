using System;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.ExternalLogin
{
    public interface IExternalLoginService
    {
        Task<Type> GetParameterTypeAsync(string providerId);

        Task<SignInResponse> SignInAsync(string providerId, object parameter);
    }
}
