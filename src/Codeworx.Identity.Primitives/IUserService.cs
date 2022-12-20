using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IUserService
    {
        Task<IUser> GetUserByExternalIdAsync(string provider, string nameIdentifier);

        Task<IUser> GetUserByIdentityAsync(ClaimsIdentity user);

        Task<IUser> GetUserByIdAsync(string userId);

        Task<IUser> GetUserByNameAsync(string userName);

        Task<string> GetProviderValueAsync(string userId, string providerId);
    }
}