using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class MfaAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public MfaAuthenticationMiddleware(RequestDelegate next)
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
            catch (ErrorResponseException<ConfirmationResponse> ex)
            {
                await context.GetResponseBinder<ConfirmationResponse>().BindAsync(ex.TypedResponse, context.Response);
            }
        }
    }
}