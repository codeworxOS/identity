using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore
{
    public class LogoutMiddleware
    {
        private readonly RequestDelegate _next;

        public LogoutMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IBaseUriAccessor accessor, IOptionsSnapshot<IdentityOptions> options, IRequestBinder<LogoutRequest> requestBinder, IResponseBinder<LogoutResponse> responseBinder)
        {
            var request = await requestBinder.BindAsync(context.Request);
            var returnUrl = request.ReturnUrl;
            if (returnUrl == null)
            {
                var builder = new UriBuilder(accessor.BaseUri.ToString());
                builder.AppendPath(options.Value.AccountEndpoint);
                builder.AppendPath("login");
                returnUrl = builder.ToString();
            }

            var response = new LogoutResponse(returnUrl);

            await responseBinder.BindAsync(response, context.Response);
        }
    }
}