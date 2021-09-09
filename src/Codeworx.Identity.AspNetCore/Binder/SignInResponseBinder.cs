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

        public override async Task BindAsync(SignInResponse responseData, HttpResponse response)
        {
            var principal = new ClaimsPrincipal(responseData.Identity);
            var returnUrl = responseData.ReturnUrl;

            await _handler.SignInAsync(response.HttpContext, principal);

            if (responseData.Identity.HasClaim(Constants.Claims.ForceChangePassword, "true"))
            {
                var builder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
                builder.AppendPath($"{_options.AccountEndpoint}/change-password");
                if (returnUrl != null)
                {
                    builder.AppendQueryParameter(Constants.ReturnUrlParameter, returnUrl);
                }

                returnUrl = builder.ToString();
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