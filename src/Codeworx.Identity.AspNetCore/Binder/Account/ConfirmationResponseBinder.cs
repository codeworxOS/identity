using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Codeworx.Identity.View;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class ConfirmationResponseBinder : ResponseBinder<ConfirmationResponse>
    {
        private readonly IConfirmationViewTemplateCache _cache;
        private readonly IContentTypeLookup _lookup;
        private readonly IdentityOptions _options;

        public ConfirmationResponseBinder(IConfirmationViewTemplateCache cache, IContentTypeLookup lookup, IOptionsSnapshot<IdentityOptions> options)
        {
            _cache = cache;
            _lookup = lookup;
            _options = options.Value;
        }

        public override async Task BindAsync(ConfirmationResponse responseData, HttpResponse response)
        {
            await response.HttpContext.SignOutAsync(_options.AuthenticationScheme).ConfigureAwait(false);

            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            var html = await _cache.GetConfirmationView(response.GetViewContextData(responseData)).ConfigureAwait(false);

            response.StatusCode = StatusCodes.Status200OK;
            await response.WriteAsync(html);
        }
    }
}
