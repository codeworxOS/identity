using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IUserService
    {
        Task<IUser> GetUserByExternalIdAsync(string provider, string nameIdentifier);

        Task<IUser> GetUserByIdentifierAsync(ClaimsIdentity user);

        Task<IUser> GetUserByNameAsync(string userName);
    }
}