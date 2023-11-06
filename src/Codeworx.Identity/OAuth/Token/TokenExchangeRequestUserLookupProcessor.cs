using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Response;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    public class TokenExchangeRequestUserLookupProcessor : IIdentityRequestProcessor<ITokenExchangeParameters, TokenExchangeRequest>
    {
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly IClientService _clientService;
        private readonly IIdentityService _identityService;
        private readonly IUserService _userService;
        private readonly ITokenProviderService _tokenProviderService;

        public TokenExchangeRequestUserLookupProcessor(
            IClientAuthenticationService clientAuthenticationService,
            IClientService clientService,
            IIdentityService identityService,
            IUserService userService,
            ITokenProviderService tokenProviderService)
        {
            _clientAuthenticationService = clientAuthenticationService;
            _clientService = clientService;
            _identityService = identityService;
            _userService = userService;
            _tokenProviderService = tokenProviderService;
        }

        public int SortOrder => 250;

        public async Task ProcessAsync(IIdentityDataParametersBuilder<ITokenExchangeParameters> builder, TokenExchangeRequest request)
        {
            var client = await _clientAuthenticationService.AuthenticateClient(request.ClientId, request.ClientSecret)
                                                                      .ConfigureAwait(false);

            var parameters = builder.Parameters;

            if (!string.IsNullOrWhiteSpace(request.Audience))
            {
                var audience = await _clientService.GetById(request.Audience).ConfigureAwait(false);
                if (audience == null)
                {
                    builder.Throw(Constants.OAuth.Error.InvalidTarget, Constants.OAuth.AudienceName);
                }

                builder.WithClient(audience);
            }
            else
            {
                builder.WithClient(client);
            }

            if (parameters.SubjectTokenType != Constants.TokenExchange.TokenType.AccessToken)
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.SubjectTokenTypeName);
            }

            var token = await _tokenProviderService.CreateAccessTokenAsync(client).ConfigureAwait(false);

            try
            {
                await token.ParseAsync(parameters.SubjectToken).ConfigureAwait(false);
            }
            catch (CacheEntryNotFoundException)
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, null);
            }

            var userId = token.IdentityData.Identifier;
            var tokenAudience = token.IdentityData.ClientId;

            if (tokenAudience != client.ClientId)
            {
                builder.Throw(Constants.OAuth.Error.InvalidGrant, null);
            }

            var user = await _userService.GetUserByIdAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.SubjectTokenName);
            }

            var claimsIdentity = await _identityService.GetClaimsIdentityFromUserAsync(user);

            var amrClaims = token.IdentityData.Claims.Where(p => p.Type.Count() == 1 && p.Type.First() == Constants.Claims.Amr).SelectMany(p => p.Values).ToList();

            if (amrClaims.Any())
            {
                foreach (var item in amrClaims)
                {
                    claimsIdentity.AddClaim(new System.Security.Claims.Claim(Constants.Claims.Amr, item));
                }
            }

            builder.WithUser(claimsIdentity, user);
        }
    }
}
