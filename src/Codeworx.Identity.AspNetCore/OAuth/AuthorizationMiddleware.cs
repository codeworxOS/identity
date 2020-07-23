using System.Security.Claims;
using System.Threading.Tasks;
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
            IAuthorizationService authorizationService,
            IRequestBinder<AuthorizationRequest> authorizationRequestBinder,
            IResponseBinder<AuthorizationSuccessResponse> authorizationSuccessResponseBinder)
        {
            ClaimsIdentity claimsIdentity = null;

            if (context.User.Identity.IsAuthenticated)
            {
                claimsIdentity = context.User.Identity as ClaimsIdentity;
            }

            try
            {
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