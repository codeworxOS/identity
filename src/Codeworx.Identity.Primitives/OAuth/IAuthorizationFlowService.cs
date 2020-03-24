using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationFlowService
    {
        string SupportedAuthorizationResponseType { get; }

        Task<IAuthorizationResult> AuthorizeRequest(OAuthAuthorizationRequest request, ClaimsIdentity user);
    }
}