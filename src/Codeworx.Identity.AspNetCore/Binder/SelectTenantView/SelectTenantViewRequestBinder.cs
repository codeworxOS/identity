using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.SelectTenantView
{
    public class SelectTenantViewRequestBinder : IRequestBinder<SelectTenantViewRequest>
    {
        private readonly IRequestBinder<AuthorizationRequest> _authorizationRequestBinder;
        private readonly IIdentityAuthenticationHandler _handler;

        public SelectTenantViewRequestBinder(
            IRequestBinder<AuthorizationRequest> authorizationRequestBinder,
            IIdentityAuthenticationHandler handler)
        {
            _authorizationRequestBinder = authorizationRequestBinder;
            _handler = handler;
        }

        public async Task<SelectTenantViewRequest> BindAsync(HttpRequest request)
        {
            var authenticationResult = await _handler.AuthenticateAsync(request.HttpContext);

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
