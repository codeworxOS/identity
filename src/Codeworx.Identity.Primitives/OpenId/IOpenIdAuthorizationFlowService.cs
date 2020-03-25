using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.OpenId
{
    public interface IOpenIdAuthorizationFlowService
    {
        bool IsSupported(string responseType);

        Task<IAuthorizationResult> AuthorizeRequest(OpenIdAuthorizationRequest request, ClaimsIdentity user);
    }
}