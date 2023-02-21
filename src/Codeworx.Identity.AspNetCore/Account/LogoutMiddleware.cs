using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class LogoutMiddleware
    {
        private readonly RequestDelegate _next;

        public LogoutMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            IBaseUriAccessor accessor,
            IdentityServerOptions options,
            IRequestBinder<LogoutRequest> requestBinder,
            IRequestValidator<LogoutRequest> requestValidator,
            IResponseBinder<LogoutResponse> responseBinder)
        {
            try
            {
                var request = await requestBinder.BindAsync(context.Request).ConfigureAwait(false);
                await requestValidator.ValidateAsync(request).ConfigureAwait(false);

                var returnUrl = request.ReturnUrl;
                if (returnUrl == null)
                {
                    var builder = new UriBuilder(accessor.BaseUri.ToString());
                    builder.AppendPath(options.AccountEndpoint);
                    builder.AppendPath("login");
                    returnUrl = builder.ToString();
                }

                var response = new LogoutResponse(returnUrl);

                await responseBinder.BindAsync(response, context.Response);
            }
            catch (ErrorResponseException error)
            {
                var binder = context.GetResponseBinder(error.ResponseType);
                await binder.BindAsync(error.Response, context.Response);
                return;
            }
        }
    }
}