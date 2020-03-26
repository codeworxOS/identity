using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationService<TRequest>
        where TRequest : OAuthAuthorizationRequest
    {
        Task<IAuthorizationResult> AuthorizeRequest(TRequest request, ClaimsIdentity user);
    }
}