using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.OpenId
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
            IAuthorizationService<Identity.OpenId.AuthorizationRequest> authorizationService,
            IRequestBinder<Identity.OpenId.AuthorizationRequest> authorizationRequestBinder,
            IResponseBinder<AuthorizationSuccessResponse> authorizationSuccessResponseBinder,
            IOptionsSnapshot<IdentityOptions> options)
        {
            ClaimsIdentity claimsIdentity = null;

            var schema = options.Value.AuthenticationScheme;

            var authResponse = await context.AuthenticateAsync(schema);

            if (authResponse.Succeeded)
            {
                claimsIdentity = authResponse.Principal.Identity as ClaimsIdentity;
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
                var responseBinder = context.GetResponseBinder(error.ResponseType);
                await responseBinder.BindAsync(error.Response, context.Response);
            }
        }
    }
}