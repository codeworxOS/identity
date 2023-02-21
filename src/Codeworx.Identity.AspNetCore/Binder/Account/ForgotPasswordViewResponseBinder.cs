using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Codeworx.Identity.View;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class ForgotPasswordViewResponseBinder : ResponseBinder<ForgotPasswordViewResponse>
    {
        private readonly IForgotPasswordViewTemplateCache _cache;
        private readonly IContentTypeLookup _lookup;

        public ForgotPasswordViewResponseBinder(IForgotPasswordViewTemplateCache cache, IContentTypeLookup lookup)
        {
            _cache = cache;
            _lookup = lookup;
        }

        protected override async Task BindAsync(ForgotPasswordViewResponse responseData, HttpResponse response, bool headerOnly)
        {
            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            response.StatusCode = StatusCodes.Status200OK;

            if (!headerOnly)
            {
                var html = await _cache.GetForgotPasswordView(response.GetViewContextData(responseData));
                await response.WriteAsync(html);
            }
        }
    }
}