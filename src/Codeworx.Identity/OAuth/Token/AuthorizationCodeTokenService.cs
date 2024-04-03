using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Token;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.OAuth.Token
{
    public class AuthorizationCodeTokenService : ITokenService<AuthorizationCodeTokenRequest>
    {
        private readonly IdentityOptions _options;
        private readonly IRequestValidator<AuthorizationCodeTokenRequest> _validator;
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly ITokenProviderService _tokenProviderService;
        private readonly IAuthorizationCodeCache _cache;
        private readonly IExternalTokenCache _externalTokenCache;

        public AuthorizationCodeTokenService(
            IAuthorizationCodeCache cache,
            IRequestValidator<AuthorizationCodeTokenRequest> validator,
            IClientAuthenticationService clientAuthenticationService,
            ITokenProviderService tokenProviderService,
            IOptionsSnapshot<IdentityOptions> options,
            IExternalTokenCache externalTokenCache = null)
        {
            _options = options.Value;
            _validator = validator;
            _clientAuthenticationService = clientAuthenticationService;
            _tokenProviderService = tokenProviderService;
            _cache = cache;
            _externalTokenCache = externalTokenCache;
        }

        public async Task<TokenResponse> ProcessAsync(AuthorizationCodeTokenRequest request, CancellationToken token = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            await _validator.ValidateAsync(request).ConfigureAwait(false);

            var client = await _clientAuthenticationService.AuthenticateClient(request.ClientId, request.ClientSecret)
                                                                                 .ConfigureAwait(false);

            // TODO implement ClientType
            ////if (!client.SupportedFlow.Any(p => p.IsSupported(request.GrantType)))
            ////{
            ////    ErrorResponse.Throw(Constants.OAuth.Error.UnauthorizedClient);
            ////}

            var identityData = await _cache.GetAsync(request.Code)
                .ConfigureAwait(false);

            if (identityData == null)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }

            if (identityData.ClientId != request.ClientId)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }

            var accessToken = await _tokenProviderService.CreateAccessTokenAsync(client).ConfigureAwait(false);
            var identityToken = await _tokenProviderService.CreateIdentityTokenAsync(client).ConfigureAwait(false);

            var validUntil = DateTimeOffset.UtcNow.Add(client.TokenExpiration);

            await accessToken.SetPayloadAsync(identityData, validUntil)
                    .ConfigureAwait(false);
            await identityToken.SetPayloadAsync(identityData, validUntil)
                    .ConfigureAwait(false);

            var scopeClaim = identityData.Claims.FirstOrDefault(p => p.Type.First() == Constants.OAuth.ScopeName);

            var scope = string.Empty;

            string refreshTokenValue = null;

            if (scopeClaim != null)
            {
                scope = string.Join(" ", scopeClaim.Values);

                if (scopeClaim.Values.Contains(Constants.OpenId.Scopes.OfflineAccess))
                {
                    var validFor = client.RefreshTokenExpiration ?? _options.RefreshToken.Expiration;

                    var refreshValidUntil = DateTimeOffset.UtcNow.Add(validFor);

                    var refreshToken = await _tokenProviderService.CreateRefreshTokenAsync(token).ConfigureAwait(false);

                    await refreshToken.SetPayloadAsync(identityData, refreshValidUntil, token).ConfigureAwait(false);
                    refreshTokenValue = await refreshToken.SerializeAsync(token);

                    if (!string.IsNullOrWhiteSpace(identityData.ExternalTokenKey) && _externalTokenCache != null)
                    {
                        await _externalTokenCache.ExtendAsync(identityData.ExternalTokenKey, validFor, token).ConfigureAwait(false);
                    }
                }
            }

            var accessTokenValue = await accessToken.SerializeAsync().ConfigureAwait(false);
            var identityTokenValue = await identityToken.SerializeAsync().ConfigureAwait(false);

            return new TokenResponse(accessTokenValue, identityTokenValue, Constants.OAuth.TokenType.Bearer, (int)client.TokenExpiration.TotalSeconds, scope, refreshTokenValue);
        }
    }
}