using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationService
    {
        Task<AuthorizationSuccessResponse> AuthorizeRequest(AuthorizationRequest request, ClaimsIdentity user);
    }
}