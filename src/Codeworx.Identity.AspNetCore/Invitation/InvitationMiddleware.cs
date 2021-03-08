using System.Threading.Tasks;
using Codeworx.Identity.Invitation;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Invitation
{
    public class InvitationMiddleware
    {
        private readonly RequestDelegate _next;

        public InvitationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IRequestBinder<InvitationViewRequest> requestBinder, IResponseBinder<InvitationViewResponse> responseBinder, IInvitationViewService service)
        {
            try
            {
                var request = await requestBinder.BindAsync(context.Request);

                var response = await service.ProcessAsync(request);

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
