using System;
using System.Security.Claims;
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
            var principal = new ClaimsPrincipal(responseData.Identity);

            var returnUrl = responseData.ReturnUrl;

            await response.HttpContext.SignInAsync(_options.AuthenticationScheme, principal);

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