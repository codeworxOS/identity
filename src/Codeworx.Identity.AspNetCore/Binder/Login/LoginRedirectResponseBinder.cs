using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.Login
{
    public class LoginRedirectResponseBinder : ResponseBinder<LoginRedirectResponse>
    {
        private readonly IdentityOptions _options;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public LoginRedirectResponseBinder(IOptionsSnapshot<IdentityOptions> options, IBaseUriAccessor baseUriAccessor)
        {
            _options = options.Value;
            _baseUriAccessor = baseUriAccessor;
        }

        public override Task BindAsync(LoginRedirectResponse responseData, HttpResponse response)
        {
            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
            uriBuilder.AppendPath(_options.AccountEndpoint);
            uriBuilder.AppendPath("login");
            uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, responseData.RedirectUri);

            if (responseData.ProviderError != null)
            {
                uriBuilder.AppendQueryParameter(Constants.ProviderLoginErrorParameter, responseData.ProviderError);
            }

            response.Redirect(uriBuilder.ToString());

            return Task.CompletedTask;
        }
    }
}
