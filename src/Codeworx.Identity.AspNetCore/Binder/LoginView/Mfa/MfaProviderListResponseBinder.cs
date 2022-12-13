using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Login.Mfa;
using Codeworx.Identity.View;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.LoginView.Mfa
{
    public class MfaProviderListResponseBinder : ResponseBinder<MfaProviderListResponse>
    {
        private readonly IContentTypeLookup _lookup;
        private readonly ILoginViewTemplateCache _view;

        public MfaProviderListResponseBinder(IContentTypeLookup lookup, ILoginViewTemplateCache view)
        {
            _lookup = lookup;
            _view = view;
        }

        protected override async Task BindAsync(MfaProviderListResponse responseData, HttpResponse response, bool headerOnly)
        {
            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            response.StatusCode = StatusCodes.Status200OK;

            if (!headerOnly)
            {
                var responseBody = await _view.GetMfaOverview(response.GetViewContextData(responseData));

                await response.WriteAsync(responseBody);
            }
        }
    }
}
