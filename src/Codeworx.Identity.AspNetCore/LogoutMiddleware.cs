using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore
{
    public class LogoutMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IViewTemplate _template;

        public LogoutMiddleware(RequestDelegate next, IViewTemplate template)
        {
            _next = next;
            _template = template;
        }

        public async Task Invoke(HttpContext context, IOptionsSnapshot<IdentityOptions> options)
        {
            var hasReturnUrl = context.Request.Query.TryGetValue(Constants.ReturnUrlParameter, out var returnUrl);

            await context.SignOutAsync(options.Value.AuthenticationScheme);

            context.Response.Redirect(hasReturnUrl ? returnUrl.First() : "login");
        }
    }
}