using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Login;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IIdentityAuthenticationHandler handler)
        {
            try
            {
                var result = await handler.AuthenticateAsync(context);

                if (result.Succeeded)
                {
                    await _next(context);
                    return;
                }

                await handler.ChallengeAsync(context);
            }
            catch (ErrorResponseException<ForceChangePasswordResponse> ex)
            {
                await context.GetResponseBinder<ForceChangePasswordResponse>().BindAsync(ex.TypedResponse, context.Response);
            }
        }
    }
}