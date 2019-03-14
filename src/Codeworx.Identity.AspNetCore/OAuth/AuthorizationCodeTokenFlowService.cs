using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeTokenFlowService : ITokenFlowService
    {
        public string SupportedGrantType => Identity.OAuth.Constants.GrantType.AuthorizationCode;

        public async Task<ITokenResult> AuthorizeRequest(TokenRequest request, (string ClientId, string ClientSecret)? authorizationHeader)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            
            return new SuccessfulTokenResult(null, null);
        }
    }
}
