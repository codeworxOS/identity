using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class TenantAuthorizationRequestProcessor : IIdentityRequestProcessor<IIdentityDataParameters, object>
    {
        private readonly ITenantService _tenantService;

        public TenantAuthorizationRequestProcessor(ITenantService tenantService = null)
        {
            _tenantService = tenantService;
        }

        public int SortOrder => 400;

        public async Task ProcessAsync(IIdentityDataParametersBuilder<IIdentityDataParameters> builder, object request)
        {
            var parameters = builder.Parameters;

            var scopes = parameters.Scopes.ToList();

            if (scopes.Contains(Constants.Scopes.Tenant))
            {
                if (_tenantService == null)
                {
                    builder.Throw(Constants.OAuth.Error.InvalidScope, Constants.Scopes.Tenant);
                }

                var tenants = (await _tenantService.GetTenantsByIdentityAsync(parameters).ConfigureAwait(false)).ToList();

                var foundTenants = tenants.Where(p => scopes.Contains(p.Key)).ToList();

                var defaultTenantKey = parameters.User.FindFirst(Constants.Claims.DefaultTenant)?.Value;

                TenantInfo currentTenant = null;

                if (tenants.Count == 0)
                {
                    builder.Throw(Constants.OAuth.Error.InvalidScope, Constants.Scopes.Tenant);
                }
                else if (foundTenants.Count == 1)
                {
                    currentTenant = foundTenants[0];
                }
                else if (foundTenants.Count == 0 && tenants.Count == 1)
                {
                    currentTenant = tenants[0];
                    scopes.Add(currentTenant.Key);
                }
                else if (foundTenants.Count == 0 && tenants.Any(p => p.Key == defaultTenantKey))
                {
                    currentTenant = tenants.First(p => p.Key == defaultTenantKey);

                    scopes.Add(defaultTenantKey);
                }
                else
                {
                    if (request is AuthorizationRequest authorizationRequest)
                    {
                        throw new ErrorResponseException<MissingTenantResponse>(new MissingTenantResponse(authorizationRequest, authorizationRequest.GetRequestPath()));
                    }

                    builder.Throw(Constants.OAuth.Error.InvalidScope, Constants.Scopes.Tenant);
                }

                if (currentTenant.AuthenticationMode == Login.AuthenticationMode.Mfa && !parameters.User.HasClaim(Constants.Claims.Amr, Constants.OpenId.Amr.Mfa))
                {
                    if (parameters.MfaFlowMode == MfaFlowMode.Enabled)
                    {
                        parameters.Throw(Constants.OpenId.Error.MfaAuthenticationRequired, Constants.Scopes.Tenant);
                    }
                }
            }

            builder.WithScopes(scopes.ToArray());
        }
    }
}
