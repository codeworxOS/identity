using System;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class SignInResponseBinder : ResponseBinder<SignInResponse>
    {
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IdentityOptions _options;

        public SignInResponseBinder(IOptionsSnapshot<IdentityOptions> options, IBaseUriAccessor baseUriAccessor)
        {
            _options = options.Value;
            _baseUriAccessor = baseUriAccessor;
        }

        public override async Task BindAsync(SignInResponse responseData, HttpResponse response)
        {
            var principal = responseData.Identity.ToClaimsPrincipal();

            var returnUrl = responseData.ReturnUrl;

            if (responseData.Identity.TenantKey != null)
            {
                await response.HttpContext.SignInAsync(_options.AuthenticationScheme, principal);
            }
            else
            {
                var authProperties = new AuthenticationProperties();
                authProperties.ExpiresUtc = DateTime.UtcNow.AddMinutes(5);
                await response.HttpContext.SignInAsync(_options.MissingTenantAuthenticationScheme, principal);

                var builder = new UriBuilder(_baseUriAccessor.BaseUri);
                builder.AppendPath($"{_options.AccountEndpoint}/login");
                builder.AppendQueryPart(Constants.ReturnUrlParameter, returnUrl);
                returnUrl = builder.ToString();
            }

            if (returnUrl == null)
            {
                var builder = new UriBuilder(_baseUriAccessor.BaseUri);
                builder.AppendPath($"{_options.AccountEndpoint}/login");
                returnUrl = builder.ToString();
            }

            response.Redirect(returnUrl);
        }
    }
}