using System.Threading.Tasks;
using Codeworx.Identity.Login.OAuth;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class ExternalCallbackMiddleware
    {
        private readonly RequestDelegate _next;

        public ExternalCallbackMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IRequestBinder<ExternalCallbackRequest> requestBinder, IResponseBinder<SignInResponse> signInBinder, ILoginService loginService)
        {
            try
            {
                ExternalCallbackRequest callbackRequest = await requestBinder.BindAsync(context.Request);
                SignInResponse signInResponse = await loginService.SignInAsync(callbackRequest.ProviderId, callbackRequest.LoginRequest);
                await signInBinder.BindAsync(signInResponse, context.Response);
            }
            catch (ErrorResponseException error)
            {
                IResponseBinder binder = context.GetResponseBinder(error.ResponseType);
                await binder.BindAsync(error.Response, context.Response);
            }
        }
    }
}