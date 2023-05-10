using System.Linq;
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
            var parsedToken = builder.Parameters.ParsedRefreshToken;

            var user = await _userService.GetUserByIdAsync(parsedToken.IdentityData.Identifier).ConfigureAwait(false);
            var identity = await _identityService.GetClaimsIdentityFromUserAsync(user).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(parsedToken.IdentityData.ExternalTokenKey))
            {
                identity.AddClaim(new System.Security.Claims.Claim(Constants.Claims.ExternalTokenKey, parsedToken.IdentityData.ExternalTokenKey));
            }

            var amrClaims = parsedToken.IdentityData.Claims.Where(p => p.Type.Count() == 1 && p.Type.ElementAt(0) == Constants.Claims.Amr).SelectMany(p => p.Values).Distinct().ToList();

            foreach (var item in amrClaims)
            {
                identity.AddClaim(new System.Security.Claims.Claim(Constants.Claims.Amr, item));
            }

            builder.WithRefreshTokenUser(identity, user);
        }
    }
}
