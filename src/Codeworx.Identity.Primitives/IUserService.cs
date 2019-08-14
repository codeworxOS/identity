using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IUserService
    {
        Task<IUser> GetUserByIdentifierAsync(string identifier);

        Task<IUser> GetUserByNameAsync(string userName);
    }
}