using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.SelectTenantView
{
    public partial class SelectTenantViewResponseBinder : ResponseBinder<SelectTenantViewResponse>
    {
        private readonly IViewTemplate _view;
        private readonly IContentTypeLookup _lookup;
        private readonly ITemplateCompiler _templateCompiler;

        public SelectTenantViewResponseBinder(ITemplateCompiler templateCompiler, IViewTemplate view, IContentTypeLookup lookup)
        {
            _view = view;
            _lookup = lookup;
            _templateCompiler = templateCompiler;
        }

        public override async Task BindAsync(SelectTenantViewResponse responseData, HttpResponse response)
        {
            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            var html = await _view.GetTenantSelectionTemplate();
            var responseBody = await _templateCompiler.RenderAsync(html, responseData);

            response.StatusCode = StatusCodes.Status200OK;

            await response.WriteAsync(responseBody);
        }
    }
}
