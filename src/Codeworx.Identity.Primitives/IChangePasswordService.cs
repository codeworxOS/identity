using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IChangePasswordService
    {
        Task SetPasswordAsync(IUser user, string password);
    }
}
