using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationService<TAuthorizationRequest>
        where TAuthorizationRequest : AuthorizationRequest
    {
        Task<AuthorizationSuccessResponse> AuthorizeRequest(TAuthorizationRequest request, ClaimsIdentity user);
    }
}