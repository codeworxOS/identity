using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    public class ClientCredentialsTokenService : ITokenService<ClientCredentialsTokenRequest>
    {
        private readonly IRequestValidator<ClientCredentialsTokenRequest> _validator;
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly IEnumerable<ITokenProvider> _tokenProviders;
        private readonly IIdentityService _identityService;

        public ClientCredentialsTokenService(
            IRequestValidator<ClientCredentialsTokenRequest> validator,
            IClientAuthenticationService clientAuthenticationService,
            IEnumerable<ITokenProvider> tokenProviders,
            IIdentityService identityService)
        {
            _validator = validator;
            _clientAuthenticationService = clientAuthenticationService;
            _tokenProviders = tokenProviders;
            _identityService = identityService;
        }

        public async Task<TokenResponse> ProcessAsync(ClientCredentialsTokenRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            await _validator.ValidateAsync(request).ConfigureAwait(false);

            var client = await _clientAuthenticationService.AuthenticateClient(request.ClientId, request.ClientSecret)
                                                                                 .ConfigureAwait(false);

            if (client.User == null)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidClient);
            }

            var user = await _identityService.GetClaimsIdentityFromUserAsync(client.User).ConfigureAwait(false);

            // TODO implement ClientType
            ////if (!client.SupportedFlow.Any(p => p.IsSupported(request.GrantType)))
            ////{
            ////    ErrorResponse.Throw(Constants.OAuth.Error.UnauthorizedClient);
            ////}

            var tokenProvider = _tokenProviders.FirstOrDefault(p => p.TokenType == Constants.Token.Jwt);

            var accessToken = await tokenProvider.CreateAsync(null).ConfigureAwait(false);

            var identityParameters = new ClientCredentialsIdentityParameters(client.ClientId, user, request.Scope);

            var identityData = await _identityService.GetIdentityAsync(identityParameters).ConfigureAwait(false);

            await accessToken.SetPayloadAsync(identityData.GetTokenClaims(ClaimTarget.AccessToken), client.TokenExpiration)
                    .ConfigureAwait(false);

            var scopeClaim = identityData.Claims.FirstOrDefault(p => p.Type.First() == Constants.OAuth.ScopeName);

            string scope = null;

            if (scopeClaim != null)
            {
                scope = string.Join(" ", scopeClaim.Values);
            }

            var accessTokenValue = await accessToken.SerializeAsync().ConfigureAwait(false);

            return new TokenResponse(accessTokenValue, null, Constants.OAuth.TokenType.Bearer, (int)client.TokenExpiration.TotalSeconds, scope);
        }

        private class ClientCredentialsIdentityParameters : IIdentityDataParameters
        {
            public ClientCredentialsIdentityParameters(string clientId, ClaimsIdentity user, string scope)
            {
                ClientId = clientId;
                User = user;
                if (scope != null)
                {
                    Scopes = scope.Split(' ').Distinct().ToImmutableArray();
                }
                else
                {
                    Scopes = ImmutableArray<string>.Empty;
                }
            }

            public string ClientId { get; }

            public string Nonce => null;

            public IReadOnlyCollection<string> Scopes { get; }

            public string State => null;

            public ClaimsIdentity User { get; }
        }
    }
}