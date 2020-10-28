using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Login.Windows;
using Codeworx.Identity.View;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class WindowsChallengeResponseBinder : ResponseBinder<WindowsChallengeResponse>
    {
        private readonly ILoginViewTemplateCache _loginViewTemplateCache;
        private readonly IContentTypeLookup _lookup;

        public WindowsChallengeResponseBinder(ILoginViewTemplateCache loginViewTemplateCache, IContentTypeLookup lookup)
        {
            _loginViewTemplateCache = loginViewTemplateCache;
            _lookup = lookup;
        }

        public override async Task BindAsync(WindowsChallengeResponse responseData, HttpResponse response)
        {
            if (responseData.DoChallenge)
            {
                await response.HttpContext.ChallengeAsync(Constants.WindowsAuthenticationSchema);

                if (_lookup.TryGetContentType(".html", out var contentType))
                {
                    response.ContentType = contentType;
                }

                var view = await _loginViewTemplateCache.GetChallengeResponse(response.GetViewContextData(responseData));
                await response.WriteAsync(view);
            }
        }
    }
}