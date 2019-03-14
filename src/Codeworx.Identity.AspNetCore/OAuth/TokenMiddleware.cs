using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRequestBinder<AuthorizationCodeTokenRequest, TokenErrorResponse> _tokenRequestBinder;
        private readonly IResponseBinder<TokenErrorResponse> _authorizationErrorResponseBinder;

        public TokenMiddleware(RequestDelegate next,
                               IRequestBinder<AuthorizationCodeTokenRequest, TokenErrorResponse> tokenRequestBinder,
                               IResponseBinder<TokenErrorResponse> authorizationErrorResponseBinder)
        {
            _next = next;
            _tokenRequestBinder = tokenRequestBinder;
            _authorizationErrorResponseBinder = authorizationErrorResponseBinder;
        }

        public async Task Invoke(HttpContext context)
        {
            var bindingResult = _tokenRequestBinder.FromQuery(context.Request.Form.ToDictionary(p => p.Key, p => p.Value as IReadOnlyCollection<string>));

            if (bindingResult.Error != null)
            {
                await _authorizationErrorResponseBinder.RespondAsync(bindingResult.Error, context);
            }
            else if (bindingResult.Result != null)
            {
                await context.Response.WriteAsync("Token endpoint");
            }
        }
    }
}