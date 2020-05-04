using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class TenantsMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            IRequestBinder<SelectTenantViewRequest> selectViewBinder,
            IRequestBinder<SelectTenantViewActionRequest> selectViewActionBinder,
            IResponseBinder<SelectTenantViewResponse> selectViewResponseBinder,
            IResponseBinder<SelectTenantSuccessResponse> selectTenantSuccessResponseBinder,
            ITenantViewService tenantViewService)
        {
            try
            {
                if (HttpMethods.IsGet(context.Request.Method))
                {
                    var request = await selectViewBinder.BindAsync(context.Request);
                    var response = await tenantViewService.ShowAsync(request);
                    await selectViewResponseBinder.BindAsync(response, context.Response);
                    return;
                }
                else if (HttpMethods.IsPost(context.Request.Method))
                {
                    var request = await selectViewActionBinder.BindAsync(context.Request);
                    var response = await tenantViewService.SelectAsync(request);
                    await selectTenantSuccessResponseBinder.BindAsync(response, context.Response);
                    return;
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                    return;
                }
            }
            catch (ErrorResponseException ex)
            {
                var binder = context.GetResponseBinder(ex.ResponseType);
                await binder.BindAsync(ex.Response, context.Response);
            }

            await _next(context);
        }
    }
}