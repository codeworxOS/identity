using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Login
{
    public class LoginRedirectResponseBinder : ResponseBinder<LoginRedirectResponse>
    {
        private readonly IdentityServerOptions _options;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public LoginRedirectResponseBinder(IdentityServerOptions options, IBaseUriAccessor baseUriAccessor)
        {
            _options = options;
            _baseUriAccessor = baseUriAccessor;
        }

        protected override Task BindAsync(LoginRedirectResponse responseData, HttpResponse response, bool headerOnly)
        {
            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
            uriBuilder.AppendPath(_options.AccountEndpoint);
            uriBuilder.AppendPath("login");
            if (!string.IsNullOrWhiteSpace(responseData.RedirectUri))
            {
                uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, responseData.RedirectUri);
            }

            if (!string.IsNullOrWhiteSpace(responseData.ProviderError))
            {
                uriBuilder.AppendQueryParameter(Constants.LoginProviderErrorParameter, responseData.ProviderError);
            }

            if (!string.IsNullOrWhiteSpace(responseData.ProviderId))
            {
                uriBuilder.AppendQueryParameter(Constants.LoginProviderIdParameter, responseData.ProviderId);
            }

            response.Redirect(uriBuilder.ToString());

            return Task.CompletedTask;
        }
    }
}
