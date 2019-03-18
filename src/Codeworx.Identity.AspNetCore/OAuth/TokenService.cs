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
        private readonly IClientAuthenticator _clientAuthenticator;

        public TokenService(IEnumerable<ITokenFlowService> tokenFlowServices, IRequestValidator<TokenRequest, TokenErrorResponse> requestValidator, IClientAuthenticator clientAuthenticator)
        {
            _tokenFlowServices = tokenFlowServices;
            _requestValidator = requestValidator;
            _clientAuthenticator = clientAuthenticator;
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

            var clientAuthorizationResult = await _clientAuthenticator.AuthenticateClient(request, authorizationHeader)
                                                                .ConfigureAwait(false);
            if (clientAuthorizationResult != null)
            {
                return clientAuthorizationResult;
            }

            //ToDo: Check if client may use grantType

            //ToDo: Check grant

            //ToDo: Check scopes

            var tokenResult = await tokenFlowService.AuthorizeRequest(request, authorizationHeader)
                                                    .ConfigureAwait(false);

            return tokenResult;
        }
    }
}
