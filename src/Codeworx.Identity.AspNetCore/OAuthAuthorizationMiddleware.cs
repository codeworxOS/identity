using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class OAuthAuthorizationMiddleware : AuthenticatedMiddleware
    {
        private readonly IRequestBinder<AuthorizationRequest, AuthorizationErrorResponse> _authorizationRequestBinder;
        private readonly IResponseBinder<AuthorizationErrorResponse> _authorizationErrorResponseBinder;
        private readonly IResponseBinder<AuthorizationCodeResponse> _authorizationCodeResponseBinder;
        private readonly IAuthorizationService _authorizationService;

        public OAuthAuthorizationMiddleware(RequestDelegate next,
                                            Configuration.IdentityService service,
                                            IRequestBinder<AuthorizationRequest, AuthorizationErrorResponse> authorizationRequestBinder,
                                            IResponseBinder<AuthorizationErrorResponse> authorizationErrorResponseBinder,
                                            IResponseBinder<AuthorizationCodeResponse> authorizationCodeResponseBinder,
                                            IAuthorizationService authorizationService)
            : base(next, service)
        {
            _authorizationRequestBinder = authorizationRequestBinder;
            _authorizationErrorResponseBinder = authorizationErrorResponseBinder;
            _authorizationCodeResponseBinder = authorizationCodeResponseBinder;
            _authorizationService = authorizationService;
        }

        protected override async Task OnInvokeAsync(HttpContext context, IPrincipal principal)
        {
            var bindingResult = _authorizationRequestBinder.FromQuery(context.Request.Query.ToDictionary(p => p.Key, p => p.Value as IReadOnlyCollection<string>));

            if (bindingResult.Error != null)
            {
                await _authorizationErrorResponseBinder.RespondAsync(bindingResult.Error, context);
            }
            else if(bindingResult.Result != null)
            {
                var result = await _authorizationService.AuthorizeRequest(bindingResult.Result);

                if (result.Error != null)
                {
                    await _authorizationErrorResponseBinder.RespondAsync(result.Error, context);
                }
                else if (result.Response != null)
                {
                    await _authorizationCodeResponseBinder.RespondAsync(result.Response, context);
                }
            }
        }
    }
}