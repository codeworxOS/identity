using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.LoginView
{
    public class LoginResponseBinder : ResponseBinder<LoginResponse>
    {
        private readonly IContentTypeLookup _lookup;
        private readonly ITemplateCompiler _templateCompiler;
        private readonly IViewTemplate _view;

        public LoginResponseBinder(IViewTemplate view, IContentTypeLookup lookup, ITemplateCompiler templateCompiler)
        {
            _view = view;
            _lookup = lookup;
            _templateCompiler = templateCompiler;
        }

        public override async Task BindAsync(LoginResponse responseData, HttpResponse response)
        {
            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            var html = await _view.GetLoginTemplate();

            var responseBody = await _templateCompiler.RenderAsync(html, responseData);

            response.StatusCode = StatusCodes.Status200OK;
            await response.WriteAsync(responseBody);
        }
    }
}