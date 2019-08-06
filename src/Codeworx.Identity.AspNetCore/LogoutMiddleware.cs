using System;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Codeworx.Identity.AspNetCore
{
    public class LogoutMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Configuration.IdentityService _service;
        private readonly IViewTemplate _template;

        public LogoutMiddleware(RequestDelegate next, Configuration.IdentityService service, IViewTemplate template)
        {
            _next = next;
            _service = service;
            _template = template;
        }

        public async Task Invoke(HttpContext context)
        {
            string body = null;

            var result = await context.AuthenticateAsync();
            var hasReturnUrl = context.Request.Query.TryGetValue("returnurl", out StringValues returnUrl);

            if (result.Succeeded)
            {
                await context.SignOutAsync(_service.AuthenticationScheme);
            }

            context.Response.Redirect(hasReturnUrl ? returnUrl.First() : "login");
        }
    }
}