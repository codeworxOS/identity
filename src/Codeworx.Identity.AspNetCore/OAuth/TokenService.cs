using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class TokenService : ITokenService
    {
        private readonly IEnumerable<ITokenFlowService> _tokenFlowServices;

        public TokenService(IEnumerable<ITokenFlowService> tokenFlowServices)
        {
            _tokenFlowServices = tokenFlowServices;
        }

        public async Task<ITokenResult> AuthorizeRequest(TokenRequest request, (string ClientId, string ClientSecret)? authorizationHeader)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var tokenFlowService = _tokenFlowServices.FirstOrDefault(p => p.SupportedGrantType == request.GrantType);
            if (tokenFlowService == null)
            {
                return new UnsupportedGrantTypeResult();
            }

            var tokenResult = await tokenFlowService.AuthorizeRequest(request, authorizationHeader)
                                                    .ConfigureAwait(false);

            return tokenResult;
        }
    }
}
