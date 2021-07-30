using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
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
            ITokenService<TokenRequest> tokenService,
            IRequestBinder<TokenRequest> tokenRequestBinder,
            IResponseBinder<TokenResponse> tokenResponseBinder)
        {
            try
            {
                var tokenRequest = await tokenRequestBinder.BindAsync(context.Request).ConfigureAwait(false);
                var result = await tokenService.ProcessAsync(tokenRequest).ConfigureAwait(false);

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