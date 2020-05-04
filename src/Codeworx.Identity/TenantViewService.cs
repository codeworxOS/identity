using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity
{
    public class TenantViewService : ITenantViewService
    {
        private readonly ITenantService _tenantService;
        private readonly IDefaultTenantService _defaultTenantService;

        public TenantViewService(ITenantService tenantService, IDefaultTenantService defaultTenantService)
        {
            _tenantService = tenantService ?? throw new System.ArgumentNullException(nameof(tenantService));
            _defaultTenantService = defaultTenantService;
        }

        public async Task<SelectTenantSuccessResponse> SelectAsync(SelectTenantViewActionRequest request)
        {
            var tenants = await _tenantService.GetTenantsByIdentityAsync(request.Identity);
            if (_defaultTenantService != null && request.SetDefault)
            {
                await _defaultTenantService.SetDefaultTenantAsync(request.Identity.ToIdentityData().Identifier, request.TenantKey);
            }

            var original = request.Request;

            var authRequest = new AuthorizationRequest(
                    original.ClientId,
                    original.RedirectUri,
                    original.ResponseType,
                    $"{original.Scope} {request.TenantKey}",
                    original.State,
                    original.Nonce,
                    original.ResponseMode);

            return new SelectTenantSuccessResponse(authRequest, request.RequestPath);
        }

        public async Task<SelectTenantViewResponse> ShowAsync(SelectTenantViewRequest request)
        {
            var tenants = await _tenantService.GetTenantsByIdentityAsync(request.Identity);
            var canSetDefault = _defaultTenantService != null;

            return new SelectTenantViewResponse(tenants, canSetDefault);
        }
    }
}
