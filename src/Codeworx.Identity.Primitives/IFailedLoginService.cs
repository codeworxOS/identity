using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IFailedLoginService
    {
        Task SetFailedLoginAsync(IUser user);

        Task<IUser> ResetFailedLoginsAsync(IUser user);
    }
}