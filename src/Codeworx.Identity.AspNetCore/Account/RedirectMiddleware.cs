using System.Threading.Tasks;
using Codeworx.Identity.Account;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Account
{
    public class RedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IRequestBinder<RedirectRequest> requestBinder, IResponseBinder<RedirectViewResponse> responseBinder)
        {
            var request = await requestBinder.BindAsync(context.Request);
            var response = new RedirectViewResponse(request.Error, request.ErrorDescription);
            await responseBinder.BindAsync(response, context.Response);
        }
    }
}
