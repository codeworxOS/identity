using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IChangeUsernameService
    {
        Task<IUser> ChangeUsernameAsync(IUser user, string username, CancellationToken token = default);
    }
}
