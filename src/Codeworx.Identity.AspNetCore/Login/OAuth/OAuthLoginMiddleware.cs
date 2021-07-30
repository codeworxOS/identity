using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore
{
    public class OAuthLoginMiddleware
    {
        private readonly RequestDelegate _next;

        public OAuthLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IOptionsSnapshot<IdentityOptions> options, IRequestBinder<OAuthRedirectRequest> requestBinder, IResponseBinder<OAuthRedirectResponse> responseBinder, IOAuthLoginService oauthService)
        {
            try
            {
                var request = await requestBinder.BindAsync(context.Request);
                var result = await oauthService.RedirectAsync(request);
                await responseBinder.BindAsync(result, context.Response);
            }
            catch (ErrorResponseException error)
            {
                IResponseBinder binder = context.GetResponseBinder(error.ResponseType);
                await binder.BindAsync(error.Response, context.Response);
            }
        }
    }
}