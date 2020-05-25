using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class TenantAuthorizationRequestProcessor : IAuthorizationRequestProcessor
    {
        private readonly ITenantService _tenantService;

        public TenantAuthorizationRequestProcessor(ITenantService tenantService = null)
        {
            _tenantService = tenantService;
        }

        public async Task<IAuthorizationParametersBuilder> ProcessAsync(IAuthorizationParametersBuilder builder, AuthorizationRequest request)
        {
            var parameters = builder.Parameters;

            var scopes = parameters.Scopes.ToList();

            if (scopes.Contains(Constants.Scopes.Tenant))
            {
                if (_tenantService == null)
                {
                    AuthorizationErrorResponse.Throw(Constants.OAuth.Error.InvalidScope, Constants.Scopes.Tenant, request.State, parameters.RedirectUri);
                }

                var tenants = (await _tenantService.GetTenantsByIdentityAsync(parameters.User).ConfigureAwait(false)).ToList();

                var foundTenants = tenants.Where(p => scopes.Contains(p.Key)).ToList();

                var defaultTenantKey = parameters.User.FindFirst(Constants.Claims.DefaultTenant)?.Value;

                if (tenants.Count == 0)
                {
                    AuthorizationErrorResponse.Throw(Constants.OAuth.Error.InvalidScope, Constants.Scopes.Tenant, request.State, parameters.RedirectUri);
                }
                else if (foundTenants.Count == 1)
                {
                }
                else if (foundTenants.Count == 0 && tenants.Count == 1)
                {
                    scopes.Add(tenants[0].Key);
                }
                else if (foundTenants.Count == 0 && tenants.Any(p => p.Key == defaultTenantKey))
                {
                    scopes.Add(defaultTenantKey);
                }
                else
                {
                    throw new ErrorResponseException<MissingTenantResponse>(new MissingTenantResponse(request, request.GetRequestPath()));
                }
            }

            return builder.WithScopes(scopes.ToArray());
        }
    }
}
