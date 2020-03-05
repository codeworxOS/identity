using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore
{
    public class ProvidersMiddleware
    {
        private readonly IContentTypeLookup _lookup;
        private readonly RequestDelegate _next;

        public ProvidersMiddleware(RequestDelegate next, IContentTypeLookup lookup)
        {
            _next = next;
            _lookup = lookup;
        }

        public async Task Invoke(HttpContext context, IOptionsSnapshot<IdentityOptions> options, IRequestBinder<ProviderRequest> requestBinder, IResponseBinder<ProviderInfosResponse> responseBinder, IExternalLoginService service)
        {
            try
            {
                var request = await requestBinder.BindAsync(context.Request);
                var providerResponse = await service.GetProviderInfosAsync(request);

                await responseBinder.BindAsync(providerResponse, context.Response);
                return;
            }
            catch (ErrorResponseException ex)
            {
                var binder = context.GetResponseBinder(ex.ResponseType);
                await binder.BindAsync(ex.Response, context.Response);
                return;
            }
        }
    }
}