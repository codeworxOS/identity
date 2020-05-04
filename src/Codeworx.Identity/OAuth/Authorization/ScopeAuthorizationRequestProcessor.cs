using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class ScopeAuthorizationRequestProcessor : IAuthorizationRequestProcessor
    {
        private readonly ISystemScopeService _systemScopeService;
        private readonly IScopeService _scopeService;
        private readonly ITenantService _tenantService;

        public ScopeAuthorizationRequestProcessor(
            ISystemScopeService systemScopeService,
            IScopeService scopeService = null,
            ITenantService tenantService = null)
        {
            _systemScopeService = systemScopeService;
            _scopeService = scopeService;
            _tenantService = tenantService;
        }

        public async Task<IAuthorizationParametersBuilder> ProcessAsync(IAuthorizationParametersBuilder builder, AuthorizationRequest request)
        {
            var parameters = builder.Parameters;

            var scopes = parameters.Scopes.ToList();
            var newScopes = new List<string>();

            if (scopes.Contains(Constants.Scopes.Tenant))
            {
                scopes.Remove(Constants.Scopes.Tenant);
                newScopes.Add(Constants.Scopes.Tenant);

                if (_tenantService == null)
                {
                    AuthorizationErrorResponse.Throw(Constants.OAuth.Error.InvalidScope, Constants.Scopes.Tenant, request.State, parameters.RedirectUri);
                }

                var tenants = (await _tenantService.GetTenantsByIdentityAsync(parameters.User).ConfigureAwait(false)).ToList();

                var foundTenants = tenants.Where(p => scopes.Contains(p.Key)).ToList();

                if (tenants.Count == 0)
                {
                    AuthorizationErrorResponse.Throw(Constants.OAuth.Error.InvalidScope, Constants.Scopes.Tenant, request.State, parameters.RedirectUri);
                }
                else if (foundTenants.Count == 1)
                {
                    scopes.Remove(foundTenants[0].Key);
                    newScopes.Add(foundTenants[0].Key);
                }
                else if (foundTenants.Count == 0 && tenants.Count == 1)
                {
                    newScopes.Add(tenants[0].Key);
                }
                else
                {
                    throw new ErrorResponseException<MissingTenantResponse>(new MissingTenantResponse(request, request.GetRequestPath()));
                }
            }

            var availableScopes = new List<string>();

            var systemScopes = await _systemScopeService.GetScopes().ConfigureAwait(false);

            availableScopes.AddRange(systemScopes.Select(p => p.ScopeKey));

            if (_scopeService != null)
            {
                var customScopes = await _scopeService.GetScopes().ConfigureAwait(false);
                availableScopes.AddRange(customScopes.Select(p => p.ScopeKey));
            }

            if (scopes.Any(p => !availableScopes.Contains(p)))
            {
                AuthorizationErrorResponse.Throw(Constants.OAuth.Error.InvalidScope, null, request.State, parameters.RedirectUri);
            }

            foreach (var scope in scopes)
            {
                if (scope.Contains(":") && scopes.Contains(scope.Split(':')[0]))
                {
                    continue;
                }

                newScopes.Add(scope);
            }

            return builder.WithScopes(newScopes.ToArray());
        }
    }
}
