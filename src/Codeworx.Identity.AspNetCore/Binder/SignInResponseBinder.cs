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
        private readonly IdentityOptions _options;

        public SignInResponseBinder(IOptionsSnapshot<IdentityOptions> options)
        {
            _options = options.Value;
        }

        public override async Task BindAsync(SignInResponse responseData, HttpResponse response)
        {
            var principal = responseData.Identity.ToClaimsPrincipal();

            if (responseData.Identity.TenantKey != null)
            {
                await response.HttpContext.SignInAsync(_options.AuthenticationScheme, principal);
            }
            else
            {
                var authProperties = new AuthenticationProperties();
                authProperties.ExpiresUtc = DateTime.UtcNow.AddMinutes(5);
                await response.HttpContext.SignInAsync(_options.MissingTenantAuthenticationScheme, principal);
            }

            response.Redirect(responseData.ReturnUrl);
        }
    }
}