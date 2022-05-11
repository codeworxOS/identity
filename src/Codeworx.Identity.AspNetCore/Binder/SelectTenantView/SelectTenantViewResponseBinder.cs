using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Codeworx.Identity.View;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.SelectTenantView
{
    public partial class SelectTenantViewResponseBinder : ResponseBinder<SelectTenantViewResponse>
    {
        private readonly ITenantViewTemplateCache _view;
        private readonly IContentTypeLookup _lookup;

        public SelectTenantViewResponseBinder(ITenantViewTemplateCache view, IContentTypeLookup lookup)
        {
            _view = view;
            _lookup = lookup;
        }

        protected override async Task BindAsync(SelectTenantViewResponse responseData, HttpResponse response, bool headerOnly)
        {
            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            response.StatusCode = StatusCodes.Status200OK;

            if (!headerOnly)
            {
                var html = await _view.GetTenantSelection(response.GetViewContextData(responseData));

                await response.WriteAsync(html);
            }
        }
    }
}
