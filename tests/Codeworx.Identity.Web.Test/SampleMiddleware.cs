using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.Web.Test
{
    public class SampleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuthenticationSchemeProvider _provider;

        public SampleMiddleware(RequestDelegate next, IAuthenticationSchemeProvider provider)
        {
            _next = next;
            _provider = provider;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Method == HttpMethod.Get.Method)
            {
                var identity = new ClaimsIdentity();
                identity.AddClaim(new Claim(ClaimTypes.Name, "blabla"));
                await httpContext.SignInAsync("Whatever", new System.Security.Claims.ClaimsPrincipal(
                    identity
                ), new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                });

                await httpContext.ChallengeAsync("Negotiate");

                var schemes = await _provider.GetAllSchemesAsync();

                var test = httpContext.User as ClaimsPrincipal;

                await httpContext.Response.WriteAsync("Whatever");
            }
            else
            {
                await _next(httpContext);
            }
        }
    }
}