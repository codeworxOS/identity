using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationMiddleware
    {
        private readonly IRequestBinder<AuthorizationRequest, AuthorizationErrorResponse> _authorizationRequestBinder;
        private readonly RequestDelegate _next;
        private readonly IEnumerable<IResponseBinder> _responseBinders;

        public AuthorizationMiddleware(
                                       RequestDelegate next,
                                       IRequestBinder<AuthorizationRequest, AuthorizationErrorResponse> authorizationRequestBinder,
                                       IEnumerable<IResponseBinder> responseBinders)
        {
            _next = next;
            _authorizationRequestBinder = authorizationRequestBinder;
            _responseBinders = responseBinders;
        }

        public async Task Invoke(HttpContext context, IAuthorizationService authorizationService)
        {
            if (context.User == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            IdentityData identity = null;

            if (context.User.Identity is ClaimsIdentity claimsIdentity)
            {
                identity = claimsIdentity.ToIdentityData();
            }

            var bindingResult = _authorizationRequestBinder.FromQuery(context.Request.Query.ToDictionary(p => p.Key, p => p.Value as IReadOnlyCollection<string>));

            if (bindingResult.Error != null)
            {
                await _responseBinders.First(p => p.Supports(bindingResult.Error.GetType()))
                                      .RespondAsync(bindingResult.Error, context);
            }
            else if (bindingResult.Result != null)
            {
                var result = await authorizationService.AuthorizeRequest(bindingResult.Result, identity);

                await _responseBinders.First(p => p.Supports(result.Response.GetType()))
                                      .RespondAsync(result.Response, context);
            }
        }
    }
}