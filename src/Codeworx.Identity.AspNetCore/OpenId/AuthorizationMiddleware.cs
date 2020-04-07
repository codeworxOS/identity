using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class AuthorizationMiddleware
    {
        private readonly IRequestBinder<Identity.OpenId.AuthorizationRequest, AuthorizationErrorResponse> _authorizationRequestBinder;
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next, IRequestBinder<Identity.OpenId.AuthorizationRequest, AuthorizationErrorResponse> authorizationRequestBinder)
        {
            _next = next;
            _authorizationRequestBinder = authorizationRequestBinder;
        }

        public async Task Invoke(HttpContext context, IAuthorizationService<Identity.OpenId.AuthorizationRequest> authorizationService)
        {
            var claimsIdentity = context.User?.Identity as ClaimsIdentity;

            if (claimsIdentity == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var bindingResult = _authorizationRequestBinder.FromQuery(context.Request.Query.ToDictionary(p => p.Key, p => p.Value as IReadOnlyCollection<string>));

            if (bindingResult.Error != null)
            {
                var responseBinder = context.GetResponseBinder<AuthorizationErrorResponse>();
                await responseBinder.BindAsync(bindingResult.Error, context.Response);
            }
            else if (bindingResult.Result != null)
            {
                var result = await authorizationService.AuthorizeRequest(bindingResult.Result, claimsIdentity);

                var responseBinder = context.GetResponseBinder(result.Response.GetType());
                await responseBinder.BindAsync(result.Response, context.Response);
            }
        }
    }
}