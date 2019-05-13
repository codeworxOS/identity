using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRequestBinder<AuthorizationRequest, AuthorizationErrorResponse> _authorizationRequestBinder;
        private readonly IResponseBinder<AuthorizationErrorResponse> _authorizationErrorResponseBinder;
        private readonly IResponseBinder<AuthorizationCodeResponse> _authorizationCodeResponseBinder;
        private readonly IResponseBinder<AuthorizationTokenResponse> _authorizationTokenResponseBinder;

        public AuthorizationMiddleware(RequestDelegate next,
                                       IRequestBinder<AuthorizationRequest, AuthorizationErrorResponse> authorizationRequestBinder,
                                       IResponseBinder<AuthorizationErrorResponse> authorizationErrorResponseBinder,
                                       IResponseBinder<AuthorizationCodeResponse> authorizationCodeResponseBinder,
                                       IResponseBinder<AuthorizationTokenResponse> authorizationTokenResponseBinder)
        {
            _next = next;
            _authorizationRequestBinder = authorizationRequestBinder;
            _authorizationErrorResponseBinder = authorizationErrorResponseBinder;
            _authorizationCodeResponseBinder = authorizationCodeResponseBinder;
            _authorizationTokenResponseBinder = authorizationTokenResponseBinder;
        }

        public async Task Invoke(HttpContext context, AuthenticatedUserInformation authenticatedUserInformation, IAuthorizationService authorizationService)
        {
            if (authenticatedUserInformation?.IdentityData == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var bindingResult = _authorizationRequestBinder.FromQuery(context.Request.Query.ToDictionary(p => p.Key, p => p.Value as IReadOnlyCollection<string>));

            if (bindingResult.Error != null)
            {
                await _authorizationErrorResponseBinder.RespondAsync(bindingResult.Error, context);
            }
            else if (bindingResult.Result != null)
            {
                var result = await authorizationService.AuthorizeRequest(bindingResult.Result, authenticatedUserInformation.IdentityData.Identifier, authenticatedUserInformation.IdentityData.TenantKey);

                if (result.Response is AuthorizationErrorResponse errorResponse)
                {
                    await _authorizationErrorResponseBinder.RespondAsync(errorResponse, context);
                }
                else if (result.Response is AuthorizationCodeResponse codeResponse)
                {
                    await _authorizationCodeResponseBinder.RespondAsync(codeResponse, context);
                }
                else if (result.Response is AuthorizationTokenResponse tokenResponse)
                {
                    await _authorizationTokenResponseBinder.RespondAsync(tokenResponse, context);
                }
            }
        }
    }
}