using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.Primitives;
using Codeworx.Identity.Configuration;
using Microsoft.AspNetCore.Authentication;

namespace Codeworx.Identity.AspNetCore
{
    public class WindowsLoginMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly Configuration.IdentityService _service;

        public WindowsLoginMiddleware(RequestDelegate next, Configuration.IdentityService service, IAuthenticationSchemeProvider schemeProvider)
        {
            _next = next;
            _service = service;
            _schemeProvider = schemeProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            var schemes = await _schemeProvider.GetAllSchemesAsync();

            if (!schemes.Any(p => p.Name.Equals(Constants.WindowsAuthenticationSchema, StringComparison.OrdinalIgnoreCase)))
            {
                context.Response.StatusCode = StatusCodes.Status406NotAcceptable;
                await context.Response.WriteAsync("Windows Authentication is disabled!");
                return;
            }

            string returnUrl = null;
            if (context.Request.Query.TryGetValue(Constants.ReturnUrlParameter, out StringValues returnUrlValue))
            {
                returnUrl = returnUrlValue.FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                context.Response.StatusCode = StatusCodes.Status406NotAcceptable;
                await context.Response.WriteAsync("ReturnUrl parameter missing");
                return;
            }

            var result = await context.AuthenticateAsync(Constants.WindowsAuthenticationSchema);

            if (result.Succeeded)
            {
                var identity = new ClaimsIdentity();
                identity.AddClaim(new Claim(ClaimTypes.Name, "Windows blabla"));

                await context.SignInAsync(
                    _service.AuthenticationScheme,
                    new System.Security.Claims.ClaimsPrincipal(
                       identity
                    ),
                    new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = DateTime.UtcNow.Add(_service.Options.CookieExpiration),
                        RedirectUri = returnUrl
                    });

                context.Response.Redirect(returnUrl);
                return;
            }
            else if (result.Failure == null)
            {
                await context.ChallengeAsync(Constants.WindowsAuthenticationSchema);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}