using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Token
{
    public class ClientCredentialsTokenRequestUserLookupProcessor : IIdentityRequestProcessor<IClientCredentialsParameters, ClientCredentialsTokenRequest>
    {
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly IIdentityService _identityService;

        public ClientCredentialsTokenRequestUserLookupProcessor(IClientAuthenticationService clientAuthenticationService, IIdentityService identityService)
        {
            _clientAuthenticationService = clientAuthenticationService;
            _identityService = identityService;
        }

        public int SortOrder => 250;

        public async Task ProcessAsync(IIdentityDataParametersBuilder<IClientCredentialsParameters> builder, ClientCredentialsTokenRequest request)
        {
            var client = await _clientAuthenticationService.AuthenticateClient(request.ClientId, request.ClientSecret)
                                                                      .ConfigureAwait(false);

            builder.WithClient(client);

            if (client.User == null)
            {
                builder.Throw(Constants.OAuth.Error.InvalidClient, "No service account assigned.");
            }

            var user = await _identityService.GetClaimsIdentityFromUserAsync(client.User).ConfigureAwait(false);

            builder.WithUser(user);
        }
    }
}
