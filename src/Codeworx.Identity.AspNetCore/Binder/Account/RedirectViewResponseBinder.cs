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

        public override async Task BindAsync(RedirectViewResponse responseData, HttpResponse response)
        {
            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            var html = await _cache.GetRedirectView(response.GetViewContextData(responseData));

            response.StatusCode = StatusCodes.Status200OK;
            await response.WriteAsync(html);
        }
    }
}
