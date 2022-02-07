using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    public class TokenExchangeRequestUserLookupProcessor : IIdentityRequestProcessor<ITokenExchangeParameters, TokenExchangeRequest>
    {
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly IClientService _clientService;
        private readonly IIdentityService _identityService;
        private readonly IUserService _userService;
        private readonly IEnumerable<ITokenProvider> _tokenProviders;

        public TokenExchangeRequestUserLookupProcessor(
            IClientAuthenticationService clientAuthenticationService,
            IClientService clientService,
            IIdentityService identityService,
            IUserService userService,
            IEnumerable<ITokenProvider> tokenProviders)
        {
            _clientAuthenticationService = clientAuthenticationService;
            _clientService = clientService;
            _identityService = identityService;
            _userService = userService;
            _tokenProviders = tokenProviders;
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

            var tokenProvider = _tokenProviders.First(p => p.TokenType == Constants.Token.Jwt);
            var token = await tokenProvider.CreateAsync(null).ConfigureAwait(false);
            await token.ParseAsync(parameters.SubjectToken).ConfigureAwait(false);

            // TODO token.ValidateAsync();
            var payload = await token.GetPayloadAsync().ConfigureAwait(false);

            var userId = (string)payload[Constants.Claims.Subject];
            var tokenAudience = (string)payload[Constants.Claims.Audience];

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

            builder.WithUser(claimsIdentity);
        }
    }
}
