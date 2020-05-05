using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Stubble.Core.Builders;

namespace Codeworx.Identity.AspNetCore.Binder.LoginView
{
    public class LoginResponseBinder : ResponseBinder<LoginResponse>
    {
        private readonly IContentTypeLookup _lookup;
        private readonly IdentityOptions _options;
        private readonly IViewTemplate _view;

        public LoginResponseBinder(IViewTemplate view, IContentTypeLookup lookup, IOptionsSnapshot<IdentityOptions> options)
        {
            _view = view;
            _lookup = lookup;
            _options = options.Value;
        }

        public override async Task BindAsync(LoginResponse responseData, HttpResponse response)
        {
            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            var html = await _view.GetLoginTemplate();
            var stubble = new StubbleBuilder()
                .Configure(p =>
                {
                    p.AddToPartialTemplateLoader(new StyleTemplateLoader(_options.Styles));
                })
                .Build();

            var responseBody = await stubble.RenderAsync(html, responseData);

            response.StatusCode = StatusCodes.Status200OK;
            await response.WriteAsync(responseBody);
        }
    }
}