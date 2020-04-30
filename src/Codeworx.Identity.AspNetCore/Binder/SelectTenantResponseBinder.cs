using System;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class SelectTenantResponseBinder : ResponseBinder<MissingTenantResponse>
    {
        private readonly IdentityOptions _options;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public SelectTenantResponseBinder(IOptionsSnapshot<IdentityOptions> options, IBaseUriAccessor baseUriAccessor)
        {
            _options = options.Value;
            _baseUriAccessor = baseUriAccessor;
        }

        public override Task BindAsync(MissingTenantResponse responseData, HttpResponse response)
        {
            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri);
            uriBuilder.AppendPath($"{_options.SelectTenantEndpoint}");
            responseData.Request.Append(uriBuilder);

            response.Redirect(uriBuilder.ToString());

            return Task.CompletedTask;
        }
    }
}
