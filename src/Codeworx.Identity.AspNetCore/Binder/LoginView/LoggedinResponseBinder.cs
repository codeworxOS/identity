using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Codeworx.Identity.View;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.LoginView
{
    public class LoggedinResponseBinder : ResponseBinder<LoggedinResponse>
    {
        private readonly IContentTypeLookup _lookup;
        private readonly ILoginViewTemplate _view;
        private readonly ITemplateCompiler _templateCompiler;

        public LoggedinResponseBinder(ILoginViewTemplate view, ITemplateCompiler templateCompiler, IContentTypeLookup lookup)
        {
            _view = view;
            _templateCompiler = templateCompiler;
            _lookup = lookup;
        }

        public override async Task BindAsync(LoggedinResponse responseData, HttpResponse response)
        {
            var template = await _view.GetLoggedInTemplate();

            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            var html = await _templateCompiler.RenderAsync(template, responseData);

            response.StatusCode = StatusCodes.Status200OK;
            await response.WriteAsync(html);
        }
    }
}