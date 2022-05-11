using System.Threading.Tasks;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class InvalidStateResponseBinder : ResponseBinder<InvalidStateResponse>
    {
        protected override async Task BindAsync(InvalidStateResponse responseData, HttpResponse response, bool headerOnly)
        {
            response.StatusCode = StatusCodes.Status406NotAcceptable;
            await response.WriteAsync(responseData.Reason);
        }
    }
}