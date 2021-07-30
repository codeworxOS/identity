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

        public override async Task BindAsync(SelectTenantViewResponse responseData, HttpResponse response)
        {
            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            var html = await _view.GetTenantSelection(response.GetViewContextData(responseData));

            response.StatusCode = StatusCodes.Status200OK;

            await response.WriteAsync(html);
        }
    }
}
