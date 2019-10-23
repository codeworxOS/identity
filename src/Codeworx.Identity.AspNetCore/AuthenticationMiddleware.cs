using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Configuration.IdentityService _service;

        public AuthenticationMiddleware(RequestDelegate next, Configuration.IdentityService service)
        {
            _next = next;
            _service = service;
        }

        public async Task Invoke(HttpContext context)
        {
            var result = await context.AuthenticateAsync(_service.Options.AuthenticationScheme);

            if (result.Succeeded)
            {
                await _next(context);
                return;
            }

            await context.ChallengeAsync(_service.Options.AuthenticationScheme);
        }
    }
}