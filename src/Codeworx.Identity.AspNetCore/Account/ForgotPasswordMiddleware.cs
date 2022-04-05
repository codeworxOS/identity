using System.Threading.Tasks;
using Codeworx.Identity.Account;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Account
{
    public class ForgotPasswordMiddleware
    {
        private readonly RequestDelegate _next;

        public ForgotPasswordMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IForgotPasswordService service, IRequestBinder<ForgotPasswordRequest> requestBinder, IResponseBinder<ForgotPasswordViewResponse> viewResponseBinder, IResponseBinder<ForgotPasswordResponse> responseBinder)
        {
            try
            {
                object response = null;
                var request = await requestBinder.BindAsync(context.Request);
                IResponseBinder binder = null;

                switch (request)
                {
                    case ProcessForgotPasswordRequest processForgotPasswordRequest:
                        response = await service.ProcessForgotPasswordAsync(processForgotPasswordRequest);
                        binder = responseBinder;
                        break;

                    case ForgotPasswordRequest forgotPasswordRequest:
                        response = await service.ShowForgotPasswordViewAsync(forgotPasswordRequest);
                        binder = viewResponseBinder;
                        break;
                    default:
                        break;
                }

                await binder.BindAsync(response, context.Response);
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
