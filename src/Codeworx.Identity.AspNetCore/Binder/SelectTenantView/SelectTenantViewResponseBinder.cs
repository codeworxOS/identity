using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Stubble.Core.Builders;
using Stubble.Core.Interfaces;

namespace Codeworx.Identity.AspNetCore.Binder.SelectTenantView
{
    public class SelectTenantViewResponseBinder : ResponseBinder<SelectTenantViewResponse>
    {
        private readonly IdentityOptions _options;
        private readonly IViewTemplate _view;
        private readonly IContentTypeLookup _lookup;

        public SelectTenantViewResponseBinder(IViewTemplate view, IContentTypeLookup lookup, IOptionsSnapshot<IdentityOptions> options)
        {
            _options = options.Value;
            _view = view;
            _lookup = lookup;
        }

        public override async Task BindAsync(SelectTenantViewResponse responseData, HttpResponse response)
        {
            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            var html = await _view.GetTenantSelectionTemplate();
            var stubble = new StubbleBuilder()
                .Configure(p => p.AddToTemplateLoader(new StyleTemplateLoader(_options.Styles)))
                .Build();

            var responseBody = await stubble.RenderAsync(html, responseData);
            response.StatusCode = StatusCodes.Status200OK;

            await response.WriteAsync(responseBody);
        }

        internal class StyleTemplateLoader : IStubbleLoader
        {
            private readonly ImmutableList<string> _styles;

            public StyleTemplateLoader(IEnumerable<string> styles)
            {
                _styles = styles.ToImmutableList();
            }

            public IStubbleLoader Clone()
            {
                return new StyleTemplateLoader(_styles);
            }

            public string Load(string name)
            {
                if (name == "Styles")
                {
                    return string.Join("\r\n", _styles.Select(p => $"<link type=\"text/css\" rel=\"stylesheet\" href=\"{p}\" >"));
                }

                return null;
            }

            public ValueTask<string> LoadAsync(string name)
            {
                return new ValueTask<string>(name);
            }
        }
    }
}
