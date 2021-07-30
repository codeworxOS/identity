using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Login
{
    public interface IExternalLoginEvent
    {
        Task BeginLoginAsync(IExternalLoginData data);

        Task UnknownLoginAsync(IExternalLoginData data);

        Task LoginSuccessAsync(IExternalLoginData data, IUser user);
    }
}
