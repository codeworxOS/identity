using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Codeworx.Identity.View;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.LoginView
{
    public class LoginResponseBinder : ResponseBinder<LoginResponse>
    {
        private readonly IContentTypeLookup _lookup;
        private readonly ILoginViewTemplateCache _view;

        public LoginResponseBinder(ILoginViewTemplateCache view, IContentTypeLookup lookup)
        {
            _view = view;
            _lookup = lookup;
        }

        public override async Task BindAsync(LoginResponse responseData, HttpResponse response)
        {
            var registrations = responseData.Groups.SelectMany(p => p.Registrations).ToList();
            if (registrations.Count == 1 && registrations[0].HasRedirectUri(out var redirectUri) && !responseData.HasError && string.IsNullOrWhiteSpace(registrations[0].Error))
            {
                response.Redirect(redirectUri);
                return;
            }

            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            var responseBody = await _view.GetLoginView(response.GetViewContextData(responseData));

            response.StatusCode = StatusCodes.Status200OK;
            await response.WriteAsync(responseBody);
        }
    }
}