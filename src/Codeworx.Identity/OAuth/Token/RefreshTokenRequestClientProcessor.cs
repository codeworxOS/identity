using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    public class RefreshTokenRequestClientProcessor : IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>
    {
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly ITokenProviderService _tokenProviderService;

        public RefreshTokenRequestClientProcessor(
            IClientAuthenticationService clientAuthenticationService,
            ITokenProviderService tokenProviderService)
        {
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

                // TODO extend refresh_token lifetime or recreate;
                builder.WithParsedRefreshToken(refreshToken);
            }
            catch (CacheEntryNotFoundException)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }
        }
    }
}
