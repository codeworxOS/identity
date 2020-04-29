using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Login
{
    public interface ILoginViewService
    {
        Task<LoggedinResponse> ProcessLoggedinAsync(LoggedinRequest request);

        Task<LoginResponse> ProcessLoginAsync(LoginRequest request);

        Task<SignInResponse> ProcessLoginFormAsync(LoginFormRequest request);
    }
}