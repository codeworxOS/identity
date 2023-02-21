using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class PasswordChangeResponseBinder : ResponseBinder<PasswordChangeResponse>
    {
        private readonly IdentityServerOptions _options;
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IIdentityAuthenticationHandler _handler;

        public PasswordChangeResponseBinder(
            IdentityServerOptions options,
            IBaseUriAccessor baseUriAccessor,
            IIdentityAuthenticationHandler handler)
        {
            _options = options;
            _baseUriAccessor = baseUriAccessor;
            _handler = handler;
        }

        protected override async Task BindAsync(PasswordChangeResponse responseData, HttpResponse response, bool headerOnly)
        {
            var builder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
            builder.AppendPath(_options.AccountEndpoint);
            builder.AppendPath("login");

            if (!string.IsNullOrWhiteSpace(responseData.Prompt))
            {
                builder.AppendQueryParameter(Constants.OAuth.PromptName, responseData.Prompt);
            }

            if (!string.IsNullOrWhiteSpace(responseData.ReturnUrl))
            {
                builder.AppendQueryParameter(Constants.ReturnUrlParameter, responseData.ReturnUrl);
            }

            await _handler.SignOutAsync(response.HttpContext);

            response.Redirect(builder.ToString());
        }
    }
}
