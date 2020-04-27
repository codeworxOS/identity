using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class TokenService : ITokenService
    {
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly IRequestValidator<TokenRequest, ErrorResponse> _requestValidator;
        private readonly IEnumerable<ITokenResultService> _tokenResultServices;
        private readonly IAuthorizationCodeCache _cache;
        private readonly IClientService _clientService;

        public TokenService(IAuthorizationCodeCache cache, IEnumerable<ITokenResultService> tokenResultServices, IRequestValidator<TokenRequest, ErrorResponse> requestValidator, IClientAuthenticationService clientAuthenticationService, IClientService clientService)
        {
            _tokenResultServices = tokenResultServices;
            _requestValidator = requestValidator;
            _clientAuthenticationService = clientAuthenticationService;
            _clientService = clientService;
            _cache = cache;
        }

        public async Task<TokenResponse> AuthorizeRequest(
            TokenRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var validationResult = await _requestValidator.IsValid(request)
                                                          .ConfigureAwait(false);
            if (validationResult != null)
            {
                throw new ErrorResponseException<ErrorResponse>(validationResult.Error);
            }

            var tokenResultService = _tokenResultServices.FirstOrDefault(p => p.SupportedGrantType == request.GrantType);
            if (tokenResultService == null)
            {
                throw new ErrorResponseException<ErrorResponse>(new ErrorResponse(Constants.OAuth.Error.UnsupportedGrantType, string.Empty, string.Empty));
            }

            var client = await _clientAuthenticationService.AuthenticateClient(request)
                                                                                 .ConfigureAwait(false);

            if (!client.SupportedFlow.Any(p => p.IsSupported(request.GrantType)))
            {
                ErrorResponse.Throw(Constants.OAuth.Error.UnauthorizedClient);
            }

            var authorizationCodeTokenRequest = request as AuthorizationCodeTokenRequest;

            if (request == null || authorizationCodeTokenRequest == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var deserializedGrantInformation = await _cache.GetAsync(authorizationCodeTokenRequest.Code)
                .ConfigureAwait(false);

            if (deserializedGrantInformation == null)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }

            var clientId = deserializedGrantInformation[Constants.OAuth.ClientIdName];
            if (clientId != request.ClientId)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }

            var accessToken = await tokenResultService.CreateAccessToken(deserializedGrantInformation, client.TokenExpiration);
            var identityToken = await tokenResultService.CreateIdToken(deserializedGrantInformation, client.TokenExpiration);

            deserializedGrantInformation.TryGetValue(Constants.OAuth.ScopeName, out var scope);

            return new TokenResponse(accessToken, identityToken, Constants.OAuth.TokenType.Bearer, (int)client.TokenExpiration.TotalSeconds, scope);
        }
    }
}