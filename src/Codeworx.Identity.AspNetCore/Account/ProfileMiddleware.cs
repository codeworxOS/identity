using System.Threading.Tasks;
using Codeworx.Identity.Account;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class ProfileMiddleware
    {
        private readonly RequestDelegate _next;

        public ProfileMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IProfileService profileService, IRequestBinder<ProfileRequest> requestBinder, IResponseBinder<ProfileResponse> responseBinder)
        {
            try
            {
                var profileRequest = await requestBinder.BindAsync(context.Request);
                var profileResponse = await profileService.ProcessAsync(profileRequest);
                await responseBinder.BindAsync(profileResponse, context.Response);
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