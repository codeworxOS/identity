using System.Linq;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class ExternalTokenScopeAuthorizationRequestProcessor : IIdentityRequestProcessor<IIdentityDataParameters, object>
    {
        public ExternalTokenScopeAuthorizationRequestProcessor()
        {
        }

        public int SortOrder => 500;

        public Task ProcessAsync(IIdentityDataParametersBuilder<IIdentityDataParameters> builder, object request)
        {
            var parameters = builder.Parameters;

            var scopes = parameters.Scopes.ToList();

            var externalTokenScopes = new[]
            {
                Constants.Scopes.ExternalToken.All,
                Constants.Scopes.ExternalToken.AccessToken,
                Constants.Scopes.ExternalToken.IdToken,
            };

            if (scopes.Any(p => externalTokenScopes.Contains(p)))
            {
                if (!parameters.User.HasClaim(p => p.Type == Constants.Claims.ExternalTokenKey))
                {
                    builder.WithScopes(scopes.Except(externalTokenScopes).ToArray());
                }
            }

            return Task.CompletedTask;
        }
    }
}
