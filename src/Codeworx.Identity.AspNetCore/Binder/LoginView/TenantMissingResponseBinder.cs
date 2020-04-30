using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.LoginView
{
    public class TenantMissingResponseBinder : ResponseBinder<SelectTenantViewResponse>
    {
        private readonly IContentTypeLookup _lookup;
        private readonly IViewTemplate _view;

        public TenantMissingResponseBinder(IViewTemplate view, IContentTypeLookup lookup)
        {
            _view = view;
            _lookup = lookup;
        }

        public override async Task BindAsync(SelectTenantViewResponse responseData, HttpResponse response)
        {
            var html = await _view.GetTenantSelectionTemplate(responseData.ReturnUrl, responseData.CanSetDefault);

            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            response.StatusCode = StatusCodes.Status200OK;
            await response.WriteAsync(html);
        }
    }
}