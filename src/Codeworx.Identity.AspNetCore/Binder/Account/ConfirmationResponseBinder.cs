using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Codeworx.Identity.View;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class ConfirmationResponseBinder : ResponseBinder<ConfirmationResponse>
    {
        private readonly IConfirmationViewTemplateCache _cache;
        private readonly IContentTypeLookup _lookup;
        private readonly IIdentityAuthenticationHandler _authenticationHandler;
        private readonly IdentityOptions _options;

        public ConfirmationResponseBinder(
            IConfirmationViewTemplateCache cache,
            IContentTypeLookup lookup,
            IIdentityAuthenticationHandler authenticationHandler,
            IOptionsSnapshot<IdentityOptions> options)
        {
            _cache = cache;
            _lookup = lookup;
            _authenticationHandler = authenticationHandler;
            _options = options.Value;
        }

        protected override async Task BindAsync(ConfirmationResponse responseData, HttpResponse response, bool headerOnly)
        {
            if (responseData.Identity != null)
            {
                await _authenticationHandler.SignInAsync(response.HttpContext, new System.Security.Claims.ClaimsPrincipal(responseData.Identity), responseData.RememberMe);
            }
            else
            {
                await _authenticationHandler.SignOutAsync(response.HttpContext);
            }

            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            response.StatusCode = StatusCodes.Status200OK;

            if (!headerOnly)
            {
                var html = await _cache.GetConfirmationView(response.GetViewContextData(responseData)).ConfigureAwait(false);
                await response.WriteAsync(html);
            }
        }
    }
}
