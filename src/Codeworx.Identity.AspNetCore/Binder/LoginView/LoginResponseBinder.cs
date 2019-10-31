using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.LoginView
{
    public class LoginResponseBinder : ResponseBinder<LoginResponse>
    {
        private readonly IContentTypeLookup _lookup;
        private readonly IViewTemplate _view;

        public LoginResponseBinder(IViewTemplate view, IContentTypeLookup lookup)
        {
            _view = view;
            _lookup = lookup;
        }

        public override async Task BindAsync(LoginResponse responseData, HttpResponse response)
        {
            var html = await _view.GetLoginTemplate(responseData.ReturnUrl, responseData.Username, responseData.Error);

            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            response.StatusCode = StatusCodes.Status200OK;
            await response.WriteAsync(html);
        }
    }
}