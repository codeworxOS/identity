using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IOptionsSnapshot<IdentityOptions> options)
        {
            var result = await context.AuthenticateAsync(options.Value.AuthenticationScheme);

            if (result.Succeeded)
            {
                await _next(context);
                return;
            }

            await context.ChallengeAsync(options.Value.AuthenticationScheme);
        }
    }
}