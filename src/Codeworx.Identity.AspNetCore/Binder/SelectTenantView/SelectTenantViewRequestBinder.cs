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
    public class SelectTenantViewRequestBinder : IRequestBinder<SelectTenantViewRequest>
    {
        private readonly IRequestBinder<AuthorizationRequest> _authorizationRequestBinder;
        private readonly IdentityOptions _options;

        public SelectTenantViewRequestBinder(
            IRequestBinder<AuthorizationRequest> authorizationRequestBinder,
            IOptionsSnapshot<IdentityOptions> options)
        {
            _authorizationRequestBinder = authorizationRequestBinder;
            _options = options.Value;
        }

        public async Task<SelectTenantViewRequest> BindAsync(HttpRequest request)
        {
            var authenticationResult = await request.HttpContext.AuthenticateAsync(_options.AuthenticationScheme);

            if (!authenticationResult.Succeeded)
            {
                throw new ErrorResponseException<UnauthorizedResponse>(new UnauthorizedResponse());
            }

            var authorizationRequest = await _authorizationRequestBinder.BindAsync(request);
            var user = authenticationResult.Principal.Identity as ClaimsIdentity;

            request.Query.TryGetValue(Constants.OAuth.RequestPathName, out var requestPath);

            return new SelectTenantViewRequest(authorizationRequest, user, requestPath.FirstOrDefault());
        }
    }
}
