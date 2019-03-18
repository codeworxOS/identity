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
        private readonly IRequestValidator<TokenRequest, TokenErrorResponse> _requestValidator;

        public TokenService(IEnumerable<ITokenFlowService> tokenFlowServices, IRequestValidator<TokenRequest, TokenErrorResponse> requestValidator)
        {
            _tokenFlowServices = tokenFlowServices;
            _requestValidator = requestValidator;
        }

        public async Task<ITokenResult> AuthorizeRequest(TokenRequest request, (string ClientId, string ClientSecret)? authorizationHeader)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var validationResult = _requestValidator.IsValid(request);
            if (validationResult != null)
            {
                return new InvalidRequestResult(validationResult);
            }

            var tokenFlowService = _tokenFlowServices.FirstOrDefault(p => p.SupportedGrantType == request.GrantType);
            if (tokenFlowService == null)
            {
                return new UnsupportedGrantTypeResult();
            }

            //ToDo: Check client authentication

            //ToDo: Check if client may use grantType

            //ToDo: Check grant

            //ToDo: Check scopes

            var tokenResult = await tokenFlowService.AuthorizeRequest(request, authorizationHeader)
                                                    .ConfigureAwait(false);

            return tokenResult;
        }
    }
}
