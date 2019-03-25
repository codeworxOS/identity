using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
using Microsoft.Extensions.Caching.Distributed;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeTokenFlowService : ITokenFlowService
    {
        private readonly IDistributedCache _cache;

        public string SupportedGrantType => Identity.OAuth.Constants.GrantType.AuthorizationCode;

        public AuthorizationCodeTokenFlowService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<ITokenResult> AuthorizeRequest(TokenRequest request)
        {
            if (request == null || !(request is AuthorizationCodeTokenRequest authorizationCodeTokenRequest))
            {
                throw new ArgumentNullException(nameof(request));
            }
            
            //ToDo: Check grant (invalid_grant)
            var grantInformation = await _cache.GetStringAsync(authorizationCodeTokenRequest.Code)
                                                     .ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(grantInformation))
            {
                return new InvalidGrantResult();
            }

            return new SuccessfulTokenResult(null, null);
        }
    }
}
