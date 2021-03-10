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

        public async Task Invoke(
            HttpContext context,
            IRequestBinder<InvitationViewRequest> requestBinder,
            IResponseBinder<InvitationViewResponse> responseBinder,
            IResponseBinder<SignInResponse> signinResponseBinder,
            IInvitationViewService service)
        {
            try
            {
                var request = await requestBinder.BindAsync(context.Request);

                if (request is ProcessInvitationViewRequest processRequest)
                {
                    var signInResponse = await service.ProcessAsync(processRequest);
                    await signinResponseBinder.BindAsync(signInResponse, context.Response);
                    return;
                }

                var response = await service.ShowAsync(request);
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
