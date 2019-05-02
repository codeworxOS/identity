using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationCodeGenerator
    {
        Task<string> GenerateCode(AuthorizationRequest request, IUser user);
    }
}
