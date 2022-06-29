using System.Threading.Tasks;
using Codeworx.Identity.Login.Mfa;
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
            IResponseBinder<MfaLoginResponse> mfaLoginResponseBinder,
            IMfaViewService service)
        {
            try
            {
                var request = await mfaLoginRequestBinder.BindAsync(context.Request);

                var response = await service.ProcessLoginAsync(request);

                await mfaLoginResponseBinder.BindAsync(response, context.Response);
                return;
            }
            catch (ErrorResponseException error)
            {
                var binder = context.GetResponseBinder(error.ResponseType);
                await binder.BindAsync(error.Response, context.Response);
                return;
            }
        }
    }
}
