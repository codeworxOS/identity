using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class PasswordChangeResponseBinder : ResponseBinder<PasswordChangeResponse>
    {
        private readonly IdentityOptions _options;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public PasswordChangeResponseBinder(IOptionsSnapshot<IdentityOptions> options, IBaseUriAccessor baseUriAccessor)
        {
            _options = options.Value;
            _baseUriAccessor = baseUriAccessor;
        }

        public override async Task BindAsync(PasswordChangeResponse responseData, HttpResponse response)
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

            await response.HttpContext.SignOutAsync(_options.AuthenticationScheme);

            response.Redirect(builder.ToString());
        }
    }
}
