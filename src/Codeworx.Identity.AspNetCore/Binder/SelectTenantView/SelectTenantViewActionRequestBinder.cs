using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.SelectTenantView
{
    public class SelectTenantViewActionRequestBinder : IRequestBinder<SelectTenantViewActionRequest>
    {
        private readonly IRequestBinder<AuthorizationRequest> _authorizationRequestBinder;
        private readonly IDefaultTenantService _defaultTenantService;
        private readonly ITenantService _tenantService;
        private readonly IdentityOptions _options;

        public SelectTenantViewActionRequestBinder(
            IRequestBinder<AuthorizationRequest> authorizationRequestBinder,
            ITenantService tenantService,
            IDefaultTenantService defaultTenantService,
            IOptions<IdentityOptions> options)
        {
            _authorizationRequestBinder = authorizationRequestBinder;
            _defaultTenantService = defaultTenantService;
            _tenantService = tenantService;
            _options = options.Value;
        }

        public async Task<SelectTenantViewActionRequest> BindAsync(HttpRequest request)
        {
            var authenticationResult = await request.HttpContext.AuthenticateAsync(_options.AuthenticationScheme);

            if (!authenticationResult.Succeeded)
            {
                throw new ErrorResponseException<UnauthorizedResponse>(new UnauthorizedResponse());
            }

            var authorizationRequest = await _authorizationRequestBinder.BindAsync(request);
            var user = authenticationResult.Principal.Identity as ClaimsIdentity;

            request.Query.TryGetValue(Constants.OAuth.RequestPathName, out var requestPath);

            var form = await request.ReadFormAsync()
                                      .ConfigureAwait(false);

            form.TryGetValue("tenantKey", out var tenantKey);
            form.TryGetValue("setDefault", out var setDefault);

            return new SelectTenantViewActionRequest(authorizationRequest, user, requestPath.FirstOrDefault(), tenantKey.FirstOrDefault(), setDefault.FirstOrDefault() == "on");
        }
    }
}
