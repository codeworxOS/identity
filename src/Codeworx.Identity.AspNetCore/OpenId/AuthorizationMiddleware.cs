using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuthorizationService authorizationService, IRequestBinder<Identity.OpenId.AuthorizationRequest> authorizationRequestBinder)
        {
            var claimsIdentity = context.User?.Identity as ClaimsIdentity;

            if (claimsIdentity == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            try
            {
                var authorizationRequest = await authorizationRequestBinder.BindAsync(context.Request)
                                                                           .ConfigureAwait(false);

                var result = await authorizationService.AuthorizeRequest(authorizationRequest, claimsIdentity);

                var responseBinder = context.GetResponseBinder(result.Response.GetType());
                await responseBinder.BindAsync(result.Response, context.Response);
            }
            catch (ErrorResponseException error)
            {
                var responseBinder = context.GetResponseBinder(error.ResponseType);
                await responseBinder.BindAsync(error.Response, context.Response);
            }
        }
    }
}