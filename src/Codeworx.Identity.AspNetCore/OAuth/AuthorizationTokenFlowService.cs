using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationTokenFlowService: IAuthorizationFlowService
    {
        public string SupportedAuthorizationResponseType => Identity.OAuth.Constants.ResponseType.Token;

        public Task<IAuthorizationResult> AuthorizeRequest(AuthorizationRequest request, string currentTenantIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}