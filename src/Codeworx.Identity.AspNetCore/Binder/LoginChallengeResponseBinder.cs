using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class LoginChallengeResponseBinder : ResponseBinder<LoginChallengeResponse>
    {
        private readonly IdentityOptions _identityOptions;
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IIdentityAuthenticationHandler _handler;

        public LoginChallengeResponseBinder(
            IOptionsSnapshot<IdentityOptions> options,
            IBaseUriAccessor baseUriAccessor,
            IIdentityAuthenticationHandler handler)
        {
            _identityOptions = options.Value;
            _baseUriAccessor = baseUriAccessor;
            _handler = handler;
        }

        public override async Task BindAsync(LoginChallengeResponse responseData, HttpResponse response)
        {
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

            await _handler.ChallengeAsync(response.HttpContext);
        }
    }
}
