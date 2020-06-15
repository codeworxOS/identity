using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class ScopeAuthorizationRequestProcessor : IAuthorizationRequestProcessor
    {
        private readonly IScopeService _scopeService;

        public ScopeAuthorizationRequestProcessor(
            IScopeService scopeService)
        {
            _scopeService = scopeService;
        }

        public async Task<IAuthorizationParametersBuilder> ProcessAsync(IAuthorizationParametersBuilder builder, AuthorizationRequest request)
        {
            var parameters = builder.Parameters;

            var scopes = parameters.Scopes.ToList();
            var newScopes = new List<string>();

            var tempScopes = await _scopeService
                .GetScopes()
                .ConfigureAwait(false);

            var availableScopes = tempScopes.Select(p => p.ScopeKey).ToList();

            if (scopes.Any(p => !availableScopes.Contains(p)))
            {
                builder.Throw(Constants.OAuth.Error.InvalidScope, null);
            }

            foreach (var scope in scopes)
            {
                if (scope.Contains(":") && scopes.Contains(scope.Split(':')[0]))
                {
                    continue;
                }

                newScopes.Add(scope);
            }

            builder = builder.WithScopes(newScopes.ToArray());

            if (request is OpenId.AuthorizationRequest)
            {
                if (!builder.Parameters.Scopes.Contains(Constants.OpenId.Scopes.OpenId))
                {
                    builder.Throw(Constants.OAuth.Error.InvalidScope, $"The {Constants.OpenId.Scopes.OpenId} scope is missing.");
                }
            }

            return builder;
        }
    }
}
