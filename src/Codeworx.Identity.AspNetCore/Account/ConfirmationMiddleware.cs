using System.Threading.Tasks;
using Codeworx.Identity.Account;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Account
{
    public class ConfirmationMiddleware
    {
        private readonly RequestDelegate _next;

        public ConfirmationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
          HttpContext context,
          IRequestBinder<ConfirmationRequest> requestBinder,
          IConfirmationViewService confirmationService,
          IResponseBinder<ConfirmationResponse> responseBinder)
        {
            try
            {
                var request = await requestBinder.BindAsync(context.Request);
                var response = await confirmationService.ProcessAsync(request, context.RequestAborted);
                await responseBinder.BindAsync(response, context.Response);
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
