using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;

namespace Codeworx.Identity
{
    public class TenantViewService : ITenantViewService
    {
        private readonly ITenantService _tenantService;
        private readonly IUserService _userService;
        private readonly IDefaultTenantService _defaultTenantService;
        private readonly IReadOnlyList<IIdentityRequestProcessor<IAuthorizationParameters, AuthorizationRequest>> _requestProcessors;

        public TenantViewService(
            ITenantService tenantService,
            IUserService userService,
            IEnumerable<IIdentityRequestProcessor<IAuthorizationParameters, AuthorizationRequest>> requestProcessors,
            IDefaultTenantService defaultTenantService = null)
        {
            _tenantService = tenantService ?? throw new System.ArgumentNullException(nameof(tenantService));
            _userService = userService;
            _defaultTenantService = defaultTenantService;
            _requestProcessors = requestProcessors.Where(p => !(p is TenantAuthorizationRequestProcessor)).OrderBy(p => p.SortOrder).ToImmutableList();
        }

        public async Task<SelectTenantSuccessResponse> SelectAsync(SelectTenantViewActionRequest request)
        {
            if (_defaultTenantService != null && request.SetDefault)
            {
                await _defaultTenantService.SetDefaultTenantAsync(request.Identity.GetUserId(), request.TenantKey);
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
            var identityDataParameters = await GetIdentityDataParametersAsync(request.Request, request.Identity)
                                .ConfigureAwait(false);

            var tenants = await _tenantService.GetTenantsByIdentityAsync(identityDataParameters);
            var canSetDefault = _defaultTenantService != null;

            return new SelectTenantViewResponse(tenants, canSetDefault);
        }

        private async Task<IIdentityDataParameters> GetIdentityDataParametersAsync(AuthorizationRequest request, ClaimsIdentity user)
        {
            var identity = await _userService.GetUserByIdentityAsync(user).ConfigureAwait(false);

            IAuthorizationParametersBuilder builder = new AuthorizationParametersBuilder(request, user, identity);

            foreach (var processor in _requestProcessors)
            {
                await processor.ProcessAsync(builder, request).ConfigureAwait(false);
            }

            return builder.Parameters;
        }
    }
}
