using System.Threading.Tasks;
using Codeworx.Identity.Account;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.View;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class RedirectViewResponseBinder : ResponseBinder<RedirectViewResponse>
    {
        private readonly IRedirectViewTemplateCache _cache;
        private readonly IContentTypeLookup _lookup;

        public RedirectViewResponseBinder(IRedirectViewTemplateCache cache, IContentTypeLookup lookup)
        {
            _cache = cache;
            _lookup = lookup;
        }

        protected override async Task BindAsync(RedirectViewResponse responseData, HttpResponse response, bool headerOnly)
        {
            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            response.StatusCode = StatusCodes.Status200OK;

            if (!headerOnly)
            {
                var html = await _cache.GetRedirectView(response.GetViewContextData(responseData));

                await response.WriteAsync(html);
            }
        }
    }
}
