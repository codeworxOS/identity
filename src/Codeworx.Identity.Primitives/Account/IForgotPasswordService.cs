using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Account
{
    public interface IForgotPasswordService
    {
        Task<bool> IsSupportedAsync();

        Task<ForgotPasswordResponse> ProcessForgotPasswordAsync(ProcessForgotPasswordRequest request);

        Task<ForgotPasswordViewResponse> ShowForgotPasswordViewAsync(ForgotPasswordRequest request);
    }
}