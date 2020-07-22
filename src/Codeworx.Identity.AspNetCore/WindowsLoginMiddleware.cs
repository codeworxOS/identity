using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class WindowsLoginMiddleware
    {
        private readonly RequestDelegate _next;

        public WindowsLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IRequestBinder<WindowsLoginRequest> requestBinder, IResponseBinder<SignInResponse> signInBinder, ILoginService externalLogin)
        {
            try
            {
                var windowsLoginRequest = await requestBinder.BindAsync(context.Request);
                var signInResonse = await externalLogin.SignInAsync(Constants.ExternalWindowsProviderId, windowsLoginRequest);
                await signInBinder.BindAsync(signInResonse, context.Response);
            }
            catch (ErrorResponseException error)
            {
                var binder = context.GetResponseBinder(error.ResponseType);
                await binder.BindAsync(error.Response, context.Response);
            }
        }
    }
}