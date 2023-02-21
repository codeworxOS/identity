using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class MissingTenantResponseBinder : ResponseBinder<MissingTenantResponse>
    {
        private readonly IdentityServerOptions _options;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public MissingTenantResponseBinder(IdentityServerOptions options, IBaseUriAccessor baseUriAccessor)
        {
            _options = options;
            _baseUriAccessor = baseUriAccessor;
        }

        protected override Task BindAsync(MissingTenantResponse responseData, HttpResponse response, bool headerOnly)
        {
            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
            uriBuilder.AppendPath($"{_options.SelectTenantEndpoint}");
            responseData.Request.Append(uriBuilder);
            uriBuilder.AppendQueryParameter(Constants.OAuth.RequestPathName, responseData.RequestPath);

            response.Redirect(uriBuilder.ToString());

            return Task.CompletedTask;
        }
    }
}
