using System.Threading.Tasks;
using Codeworx.Identity.Login.Mfa;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Account
{
    public class MfaLoginMiddleware
    {
        private readonly RequestDelegate _next;

        public MfaLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            IRequestBinder<MfaLoginRequest> mfaLoginRequestBinder,
            IRequestValidator<MfaLoginRequest> mfaLoginRequestValidator,
            IResponseBinder<MfaLoginResponse> mfaLoginResponseBinder,
            IResponseBinder<SignInResponse> signInResponseBinder,
            IMfaViewService service)
        {
            try
            {
                var request = await mfaLoginRequestBinder.BindAsync(context.Request).ConfigureAwait(false);
                await mfaLoginRequestValidator.ValidateAsync(request).ConfigureAwait(false);

                if (request is MfaProcessLoginRequest processLoginRequest)
                {
                    try
                    {
                        var signInResponse = await service.ProcessLoginAsync(processLoginRequest).ConfigureAwait(false);
                        await signInResponseBinder.BindAsync(signInResponse, context.Response).ConfigureAwait(false);
                        return;
                    }
                    catch (AuthenticationException ex)
                    {
                        var showResponse = await service.ShowLoginAsync(request, processLoginRequest.ProviderId, ex.Message).ConfigureAwait(false);
                        await mfaLoginResponseBinder.BindAsync(showResponse, context.Response).ConfigureAwait(false);
                    }
                }
                else
                {
                    var showResponse = await service.ShowLoginAsync(request).ConfigureAwait(false);
                    await mfaLoginResponseBinder.BindAsync(showResponse, context.Response).ConfigureAwait(false);
                    return;
                }
            }
            catch (ErrorResponseException error)
            {
                var binder = context.GetResponseBinder(error.ResponseType);
                await binder.BindAsync(error.Response, context.Response).ConfigureAwait(false);
                return;
            }
        }
    }
}
