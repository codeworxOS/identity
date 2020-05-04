using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Stubble.Core.Interfaces;

namespace Codeworx.Identity.AspNetCore.Binder.SelectTenantView
{
    public class SelectTenantSuccessResponseBinder : ResponseBinder<SelectTenantSuccessResponse>
    {
        private readonly IdentityOptions _options;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public SelectTenantSuccessResponseBinder(IOptionsSnapshot<IdentityOptions> options, IBaseUriAccessor baseUriAccessor)
        {
            _options = options.Value;
            _baseUriAccessor = baseUriAccessor;
        }

        public override Task BindAsync(SelectTenantSuccessResponse responseData, HttpResponse response)
        {
            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri);
            switch (responseData.RequestPath)
            {
                case "oauth":
                    uriBuilder.AppendPath($"{_options.OauthAuthorizationEndpoint}");
                    break;
                case "openid":
                    uriBuilder.AppendPath($"{_options.OpenIdAuthorizationEndpoint}");
                    break;
                default:
                    response.StatusCode = StatusCodes.Status406NotAcceptable;
                    return Task.CompletedTask;
            }

            responseData.Request.Append(uriBuilder);

            response.Redirect(uriBuilder.ToString());

            return Task.CompletedTask;
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
