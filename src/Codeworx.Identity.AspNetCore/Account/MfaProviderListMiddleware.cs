using System.Threading.Tasks;
using Codeworx.Identity.Login.Mfa;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Account
{
    public class MfaProviderListMiddleware
    {
        private readonly RequestDelegate _next;

        public MfaProviderListMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            IRequestBinder<MfaProviderListRequest> mfaProviderListRequestBinder,
            IResponseBinder<MfaProviderListResponse> mfaProviderListResponseBinder,
            IMfaViewService service)
        {
            try
            {
                var request = await mfaProviderListRequestBinder.BindAsync(context.Request).ConfigureAwait(false);

                var showResponse = await service.ShowProviderListAsync(request).ConfigureAwait(false);
                await mfaProviderListResponseBinder.BindAsync(showResponse, context.Response).ConfigureAwait(false);
                return;
            }
            catch (ErrorResponseException error)
            {
                var binder = context.GetResponseBinder(error.ResponseType);
                await binder.BindAsync(error.Response, context.Response).ConfigureAwait(false);
                return;
            }
        }
    }
}
