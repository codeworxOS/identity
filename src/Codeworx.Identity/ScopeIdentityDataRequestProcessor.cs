using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public class ScopeIdentityDataRequestProcessor : IIdentityRequestProcessor<IIdentityDataParameters, object>
    {
        private readonly IScopeService _scopeService;

        public ScopeIdentityDataRequestProcessor(
            IScopeService scopeService)
        {
            _scopeService = scopeService;
        }

        public int SortOrder => 300;

        public async Task ProcessAsync(IIdentityDataParametersBuilder<IIdentityDataParameters> builder, object request)
        {
            var parameters = builder.Parameters;

            var scopes = parameters.Scopes.ToList();
            var newScopes = new List<string>();

            var tempScopes = await _scopeService
                .GetScopes(parameters.Client)
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
        }
    }
}
