using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.SelectTenantView
{
    public class SelectTenantSuccessResponseBinder : ResponseBinder<SelectTenantSuccessResponse>
    {
        private readonly IdentityServerOptions _options;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public SelectTenantSuccessResponseBinder(IdentityServerOptions options, IBaseUriAccessor baseUriAccessor)
        {
            _options = options;
            _baseUriAccessor = baseUriAccessor;
        }

        protected override Task BindAsync(SelectTenantSuccessResponse responseData, HttpResponse response, bool headerOnly)
        {
            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
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
    }
}
