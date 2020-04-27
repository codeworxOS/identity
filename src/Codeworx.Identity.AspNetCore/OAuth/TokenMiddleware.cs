using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            ITokenService tokenService,
            IRequestBinder<AuthorizationCodeTokenRequest> tokenRequestBinder,
            IResponseBinder<TokenResponse> tokenResponseBinder)
        {
            IRequestBindingResult<AuthorizationCodeTokenRequest, ErrorResponse> bindingResult = null;

            try
            {
                var tokenRequest = await tokenRequestBinder.BindAsync(context.Request);
                var result = await tokenService.AuthorizeRequest(bindingResult.Result);

                await tokenResponseBinder.BindAsync(result, context.Response);
            }
            catch (ErrorResponseException error)
            {
                var binder = context.GetResponseBinder(error.ResponseType);
                await binder.BindAsync(error.Response, context.Response);
            }
        }
    }
}