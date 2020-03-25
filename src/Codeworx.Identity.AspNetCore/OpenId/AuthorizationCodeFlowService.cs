using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OpenId;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class AuthorizationCodeFlowService : IOpenIdAuthorizationFlowService
    {
        public bool IsSupported(string responseType)
        {
            return Equals(responseType, Identity.OAuth.Constants.ResponseType.Code);
        }

        public Task<IAuthorizationResult> AuthorizeRequest(OpenIdAuthorizationRequest request, ClaimsIdentity user)
        {
            throw new System.NotImplementedException();
        }
    }
}