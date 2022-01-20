using System.Threading.Tasks;
using Codeworx.Identity.Cache;

namespace Codeworx.Identity.OAuth.Token
{
    public class RefreshTokenRequestClientProcessor : IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>
    {
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly IRefreshTokenCache _refreshTokenCache;

        public RefreshTokenRequestClientProcessor(IClientAuthenticationService clientAuthenticationService, IRefreshTokenCache refreshTokenCache)
        {
            _clientAuthenticationService = clientAuthenticationService;
            _refreshTokenCache = refreshTokenCache;
        }

        public int SortOrder => 250;

        public async Task ProcessAsync(IIdentityDataParametersBuilder<IRefreshTokenParameters> builder, RefreshTokenRequest request)
        {
            var client = await _clientAuthenticationService.AuthenticateClient(request.ClientId, request.ClientSecret)
                                                                                  .ConfigureAwait(false);

            if (client == null)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidClient);
            }

            builder.WithClient(client);

            // TODO implement ClientType
            ////if (!client.SupportedFlow.Any(p => p.IsSupported(request.GrantType)))
            ////{
            ////    ErrorResponse.Throw(Constants.OAuth.Error.UnauthorizedClient);
            ////}

            var cacheEntry = await _refreshTokenCache.GetAsync(request.RefreshToken).ConfigureAwait(false);

            if (cacheEntry == null)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }

            if (cacheEntry.IdentityData.ClientId != request.ClientId)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }

            // TODO extend refresh_token lifetime or recreate;
            builder.WithCacheItem(cacheEntry);
        }
    }
}
