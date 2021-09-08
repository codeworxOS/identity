using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            IAuthorizationService<AuthorizationRequest> authorizationService,
            IRequestBinder<AuthorizationRequest> authorizationRequestBinder,
            IResponseBinder<AuthorizationSuccessResponse> authorizationSuccessResponseBinder,
            IIdentityAuthenticationHandler handler)
        {
            ClaimsIdentity claimsIdentity = null;

            try
            {
                var authResponse = await handler.AuthenticateAsync(context);

                if (authResponse.Succeeded)
                {
                    claimsIdentity = authResponse.Principal.Identity as ClaimsIdentity;
                }

                var authorizationRequest = await authorizationRequestBinder.BindAsync(context.Request)
                                                                           .ConfigureAwait(false);

                var result = await authorizationService.AuthorizeRequest(authorizationRequest, claimsIdentity).ConfigureAwait(false);

                await authorizationSuccessResponseBinder.BindAsync(result, context.Response).ConfigureAwait(false);
            }
            catch (ErrorResponseException error)
            {
                var binder = context.GetResponseBinder(error.ResponseType);
                await binder.BindAsync(error.Response, context.Response);
            }
        }
    }
}