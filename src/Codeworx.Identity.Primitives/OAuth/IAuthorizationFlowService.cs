using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationFlowService<TRequest>
        where TRequest : OAuthAuthorizationRequest
    {
        bool IsSupported(string responseType);

        Task<IAuthorizationResult> AuthorizeRequest(TRequest request, ClaimsIdentity user);
    }
}