using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Codeworx.Identity.View;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class PasswordChangeViewResponseBinder : ResponseBinder<PasswordChangeViewResponse>
    {
        private readonly IPasswordChangeViewTemplateCache _cache;
        private readonly IContentTypeLookup _lookup;

        public PasswordChangeViewResponseBinder(IPasswordChangeViewTemplateCache cache, IContentTypeLookup lookup)
        {
            _cache = cache;
            _lookup = lookup;
        }

        public override async Task BindAsync(PasswordChangeViewResponse responseData, HttpResponse response)
        {
            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            var html = await _cache.GetPasswordChangeView(response.GetViewContextData(responseData));

            response.StatusCode = StatusCodes.Status200OK;
            await response.WriteAsync(html);
        }
    }
}