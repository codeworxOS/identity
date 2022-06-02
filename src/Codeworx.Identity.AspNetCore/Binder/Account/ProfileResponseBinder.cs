using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Codeworx.Identity.View;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class ProfileResponseBinder : ResponseBinder<ProfileResponse>
    {
        private readonly IContentTypeLookup _lookup;
        private readonly IProfileViewTemplateCache _view;

        public ProfileResponseBinder(IProfileViewTemplateCache view, IContentTypeLookup lookup)
        {
            _view = view;
            _lookup = lookup;
        }

        protected override async Task BindAsync(ProfileResponse responseData, HttpResponse response, bool headerOnly)
        {
            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            response.StatusCode = StatusCodes.Status200OK;

            if (!headerOnly)
            {
                var html = await _view.GetProfileView(response.GetViewContextData(responseData));

                await response.WriteAsync(html);
            }
        }
    }
}