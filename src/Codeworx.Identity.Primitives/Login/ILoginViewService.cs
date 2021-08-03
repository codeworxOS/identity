using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Login
{
    public interface ILoginViewService
    {
        Task<LoginResponse> ProcessLoginAsync(LoginRequest request);

        Task<SignInResponse> ProcessLoginFormAsync(LoginFormRequest request);

        Task<LoggedinResponse> ProcessLoggedinAsync(LoggedinRequest loggedin);
    }
}