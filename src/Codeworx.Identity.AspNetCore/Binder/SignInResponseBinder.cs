using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class SignInResponseBinder : ResponseBinder<SignInResponse>
    {
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IIdentityAuthenticationHandler _handler;
        private readonly IdentityOptions _options;

        public SignInResponseBinder(IOptionsSnapshot<IdentityOptions> options, IBaseUriAccessor baseUriAccessor, IIdentityAuthenticationHandler handler)
        {
            _options = options.Value;
            _baseUriAccessor = baseUriAccessor;
            _handler = handler;
        }

        protected override async Task BindAsync(SignInResponse responseData, HttpResponse response, bool headerOnly)
        {
            var returnUrl = responseData.ReturnUrl;

            if (responseData.Login != null)
            {
                var identity = responseData.Login.Identity;
                var principal = new ClaimsPrincipal(identity);
                await _handler.SignInAsync(response.HttpContext, principal, responseData.Login.Remember, Identity.Login.AuthenticationMode.Login);

                if (identity.HasClaim(Constants.Claims.ForceChangePassword, "true"))
                {
                    var builder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
                    builder.AppendPath($"{_options.AccountEndpoint}/change-password");
                    if (returnUrl != null)
                    {
                        builder.AppendQueryParameter(Constants.ReturnUrlParameter, returnUrl);
                    }

                    returnUrl = builder.ToString();
                }
                else if (identity.HasClaim(Constants.Claims.ForceMfaLogin, "true") && responseData.Mfa == null)
                {
                    var builder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
                    builder.AppendPath($"{_options.AccountEndpoint}/login/mfa");
                    if (returnUrl != null)
                    {
                        builder.AppendQueryParameter(Constants.ReturnUrlParameter, returnUrl);
                    }

                    returnUrl = builder.ToString();
                }
            }

            if (responseData.Mfa != null)
            {
                var principal = new ClaimsPrincipal(responseData.Mfa.Identity);
                await _handler.SignInAsync(response.HttpContext, principal, responseData.Mfa.Remember, Identity.Login.AuthenticationMode.Mfa);
            }

            if (returnUrl == null)
            {
                var builder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
                builder.AppendPath($"{_options.AccountEndpoint}/login");
                returnUrl = builder.ToString();
            }

            response.Redirect(returnUrl);
        }
    }
}