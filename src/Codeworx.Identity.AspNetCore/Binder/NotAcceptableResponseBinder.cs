using System.Threading.Tasks;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class NotAcceptableResponseBinder : ResponseBinder<NotAcceptableResponse>
    {
        protected override async Task BindAsync(NotAcceptableResponse responseData, HttpResponse response, bool headerOnly)
        {
            response.StatusCode = StatusCodes.Status406NotAcceptable;
            await response.WriteAsync(responseData.Reason);
        }
    }
}