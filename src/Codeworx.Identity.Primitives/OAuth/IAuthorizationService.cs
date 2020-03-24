using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationService
    {
        Task<IAuthorizationResult> AuthorizeRequest(OAuthAuthorizationRequest request, ClaimsIdentity user);
    }
}