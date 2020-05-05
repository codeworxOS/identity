using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;

namespace Codeworx.Identity.OAuth.Token
{
    public class TokenService : ITokenService
    {
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly IRequestValidator<TokenRequest> _requestValidator;
        private readonly IEnumerable<ITokenResultService> _tokenResultServices;
        private readonly IAuthorizationCodeCache _cache;

        public TokenService(
            IAuthorizationCodeCache cache,
            IEnumerable<ITokenResultService> tokenResultServices,
            IRequestValidator<TokenRequest> requestValidator,
            IClientAuthenticationService clientAuthenticationService)
        {
            _tokenResultServices = tokenResultServices;
            _requestValidator = requestValidator;
            _clientAuthenticationService = clientAuthenticationService;
            _cache = cache;
        }

        public async Task<TokenResponse> AuthorizeRequest(
            TokenRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            await _requestValidator.ValidateAsync(request)
                                                          .ConfigureAwait(false);

            var tokenResultService = _tokenResultServices.FirstOrDefault(p => p.SupportedGrantType == request.GrantType);
            if (tokenResultService == null)
            {
                throw new ErrorResponseException<ErrorResponse>(new ErrorResponse(Constants.OAuth.Error.UnsupportedGrantType, string.Empty, string.Empty));
            }

            var client = await _clientAuthenticationService.AuthenticateClient(request)
                                                                                 .ConfigureAwait(false);

            // TODO implement ClientType
            ////if (!client.SupportedFlow.Any(p => p.IsSupported(request.GrantType)))
            ////{
            ////    ErrorResponse.Throw(Constants.OAuth.Error.UnauthorizedClient);
            ////}

            var authorizationCodeTokenRequest = request as AuthorizationCodeTokenRequest;

            if (request == null || authorizationCodeTokenRequest == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var identityData = await _cache.GetAsync(authorizationCodeTokenRequest.Code)
                .ConfigureAwait(false);

            if (identityData == null)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }

            if (identityData.ClientId != request.ClientId)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }

            var accessToken = await tokenResultService.CreateAccessToken(identityData, client.TokenExpiration);
            var identityToken = await tokenResultService.CreateIdToken(identityData, client.TokenExpiration);

            var scopeClaim = identityData.Claims.FirstOrDefault(p => p.Type.First() == Constants.OAuth.ScopeName);

            var scope = string.Empty;

            if (scopeClaim != null)
            {
                scope = string.Join(" ", scopeClaim.Values);
            }

            return new TokenResponse(accessToken, identityToken, Constants.OAuth.TokenType.Bearer, (int)client.TokenExpiration.TotalSeconds, scope);
        }
    }
}