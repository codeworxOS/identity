using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class LoginChallengeResponseBinder : ResponseBinder<LoginChallengeResponse>
    {
        private readonly IdentityOptions _identityOptions;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public LoginChallengeResponseBinder(IOptionsSnapshot<IdentityOptions> options, IBaseUriAccessor baseUriAccessor)
        {
            _identityOptions = options.Value;
            _baseUriAccessor = baseUriAccessor;
        }

        public override async Task BindAsync(LoginChallengeResponse responseData, HttpResponse response)
        {
            var properties = new AuthenticationProperties();
            if (!string.IsNullOrWhiteSpace(responseData.Prompt))
            {
                var loginUrlBuilder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
                loginUrlBuilder.AppendPath(_identityOptions.AccountEndpoint);
                loginUrlBuilder.AppendPath("login");
                loginUrlBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, response.HttpContext.Request.GetEncodedUrl());
                loginUrlBuilder.AppendQueryParameter(Constants.OAuth.PromptName, responseData.Prompt);

                response.Redirect(loginUrlBuilder.ToString());
                return;
            }

            await response.HttpContext.ChallengeAsync(_identityOptions.AuthenticationScheme, properties);
        }
    }
}
