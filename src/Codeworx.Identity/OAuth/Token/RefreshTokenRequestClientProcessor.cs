using System;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Token;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.OAuth.Token
{
    public class RefreshTokenRequestClientProcessor : IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>
    {
        private readonly IdentityOptions _options;
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly ITokenProviderService _tokenProviderService;

        public RefreshTokenRequestClientProcessor(
            IClientAuthenticationService clientAuthenticationService,
            ITokenProviderService tokenProviderService,
            IOptionsSnapshot<IdentityOptions> options)
        {
            _options = options.Value;
            _clientAuthenticationService = clientAuthenticationService;
            _tokenProviderService = tokenProviderService;
        }

        public int SortOrder => 250;

        public async Task ProcessAsync(IIdentityDataParametersBuilder<IRefreshTokenParameters> builder, RefreshTokenRequest request)
        {
            var client = await _clientAuthenticationService.AuthenticateClient(request.ClientId, request.ClientSecret)
                                                                                  .ConfigureAwait(false);

            builder.WithClient(client);

            // TODO implement ClientType
            ////if (!client.SupportedFlow.Any(p => p.IsSupported(request.GrantType)))
            ////{
            ////    ErrorResponse.Throw(Constants.OAuth.Error.UnauthorizedClient);
            ////}

            try
            {
                var refreshToken = await _tokenProviderService.CreateRefreshTokenAsync().ConfigureAwait(false);

                await refreshToken.ParseAsync(request.RefreshToken).ConfigureAwait(false);

                if (refreshToken.IdentityData.ClientId != request.ClientId)
                {
                    ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
                }

                var lifetime = client.RefreshTokenLifetime ?? _options.RefreshToken.Lifetime;
                var validFor = client.RefreshTokenExpiration ?? _options.RefreshToken.Expiration;
                var validUntil = DateTimeOffset.UtcNow.Add(validFor);

                switch (lifetime)
                {
                    case RefreshTokenLifetime.UseOnce:
                        await CreateNewRefreshTokenAsync(builder, refreshToken, validUntil);
                        break;
                    case RefreshTokenLifetime.SlidingExpiration:
                        await refreshToken.ExtendLifetimeAsync(validUntil);
                        builder.WithParsedRefreshToken(refreshToken);
                        break;
                    case RefreshTokenLifetime.RecreateAfterHalfLifetime:
                        var halfExpiration = TimeSpan.FromTicks(validFor.Ticks / 2);
                        if (refreshToken.ValidUntil.Add(-halfExpiration) < DateTimeOffset.UtcNow)
                        {
                            await CreateNewRefreshTokenAsync(builder, refreshToken, validUntil);
                        }
                        else
                        {
                            builder.WithParsedRefreshToken(refreshToken);
                        }

                        break;
                    default:
                        break;
                }
            }
            catch (CacheEntryNotFoundException)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }
        }

        private async Task CreateNewRefreshTokenAsync(IIdentityDataParametersBuilder<IRefreshTokenParameters> builder, IToken refreshToken, DateTimeOffset validUntil)
        {
            await refreshToken.ExtendLifetimeAsync(DateTimeOffset.UtcNow.AddMinutes(-5));
            var newToken = await _tokenProviderService.CreateRefreshTokenAsync();
            await newToken.SetPayloadAsync(refreshToken.IdentityData, validUntil);
            var newKey = await newToken.SerializeAsync();
            builder.WithRefreshToken(newKey);
            builder.WithParsedRefreshToken(newToken);
        }
    }
}
