using System.Threading.Tasks;
using Codeworx.Identity.Account;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class ProfileLinkMiddleware
    {
        private readonly RequestDelegate _next;

        public ProfileLinkMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IProfileService profileService, IRequestBinder<ProfileLinkRequest> requestBinder, IResponseBinder<ProfileLinkResponse> responseBinder)
        {
            try
            {
                var profileLinkRequest = await requestBinder.BindAsync(context.Request);
                var profileLinkResponse = await profileService.ProcessLinkAsync(profileLinkRequest);
                await responseBinder.BindAsync(profileLinkResponse, context.Response);
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