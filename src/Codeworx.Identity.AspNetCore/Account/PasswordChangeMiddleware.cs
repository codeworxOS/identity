using System;
using System.Threading.Tasks;
using Codeworx.Identity.Account;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class PasswordChangeMiddleware
    {
        private readonly RequestDelegate _next;

        public PasswordChangeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            IRequestBinder<PasswordChangeRequest> requestBinder,
            IPasswordChangeService service,
            IServiceProvider serviceProvider)
        {
            try
            {
                object response = null;
                var request = await requestBinder.BindAsync(context.Request);
                IResponseBinder responseBinder = null;

                switch (request)
                {
                    case ProcessPasswordChangeRequest passwordChangeRequest:
                        response = await service.ProcessChangePasswordAsync(passwordChangeRequest);
                        responseBinder = context.GetResponseBinder<PasswordChangeResponse>();
                        break;

                    case PasswordChangeRequest processPasswordChangeRequest:
                        response = await service.ShowChangePasswordViewAsync(processPasswordChangeRequest);
                        responseBinder = context.GetResponseBinder<PasswordChangeViewResponse>();
                        break;
                }

                await responseBinder.BindAsync(response, context.Response);
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