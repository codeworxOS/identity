using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class LogoutMiddleware
    {
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private readonly RequestDelegate _next;
        private readonly IViewTemplate _template;

        public LogoutMiddleware(RequestDelegate next, IViewTemplate template, IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _next = next;
            _template = template;
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            var hasReturnUrl = context.Request.Query.TryGetValue(Constants.ReturnUrlParameter, out var returnUrl);

            var authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();

            foreach (AuthenticationScheme authenticationScheme in authenticationSchemes)
            {
                await context.SignOutAsync(authenticationScheme.Name);
            }

            context.Response.Redirect(hasReturnUrl ? returnUrl.First() : "login");
        }
    }
}