using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Token
{
    public class RefreshTokenRequestUserProcessor : IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>
    {
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;

        public RefreshTokenRequestUserProcessor(IUserService userService, IIdentityService identityService)
        {
            _userService = userService;
            _identityService = identityService;
        }

        public int SortOrder => 350;

        public async Task ProcessAsync(IIdentityDataParametersBuilder<IRefreshTokenParameters> builder, RefreshTokenRequest request)
        {
            var cacheItem = builder.Parameters.CacheItem;

            var user = await _userService.GetUserByIdAsync(cacheItem.IdentityData.Identifier).ConfigureAwait(false);
            var identity = await _identityService.GetClaimsIdentityFromUserAsync(user).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(cacheItem.IdentityData.ExternalTokenKey))
            {
                identity.AddClaim(new System.Security.Claims.Claim(Constants.Claims.ExternalTokenKey, cacheItem.IdentityData.ExternalTokenKey));
            }

            builder.WithRefreshTokenUser(identity);
        }
    }
}
