using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
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
                var result = await handler.AuthenticateAsync(context).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    if (result.Principal.HasClaim(Constants.Claims.ForceMfaLogin, "true"))
                    {
                        var mfaResult = await handler.AuthenticateAsync(context, AuthenticationMode.Mfa);
                        if (!mfaResult.Succeeded)
                        {
                            await handler.ChallengeAsync(context, AuthenticationMode.Mfa).ConfigureAwait(false);
                            return;
                        }
                    }

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