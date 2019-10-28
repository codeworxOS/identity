using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Response
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

            await response.HttpContext.SignInAsync(_options.AuthenticationScheme, principal);
        }
    }
}
