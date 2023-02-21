using System.Linq;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Token
{
    public class RefreshTokenRequestScopeProcessor : IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>
    {
        public RefreshTokenRequestScopeProcessor()
        {
        }

        public int SortOrder => 450;

        public Task ProcessAsync(IIdentityDataParametersBuilder<IRefreshTokenParameters> builder, RefreshTokenRequest request)
        {
            var parameters = builder.Parameters;

            var scopeClaim = parameters.ParsedRefreshToken.IdentityData.Claims.FirstOrDefault(p => p.Type.First() == Constants.OAuth.ScopeName);
            var scopes = scopeClaim?.Values?.ToArray() ?? new string[] { };

            if (parameters.Scopes.Any())
            {
                var invalidScopes = parameters.Scopes.Where(p => !scopes.Contains(p)).ToList();

                if (invalidScopes.Any())
                {
                    parameters.Throw(Constants.OAuth.Error.InvalidScope, string.Join(" ", invalidScopes));
                }
            }

            if (parameters.Scopes.Any())
            {
                scopes = parameters.Scopes.ToArray();
            }

            builder.WithScopes(scopes);

            return Task.CompletedTask;
        }
    }
}
