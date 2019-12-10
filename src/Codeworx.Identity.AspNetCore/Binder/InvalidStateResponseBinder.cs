using System.Threading.Tasks;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class InvalidStateResponseBinder : ResponseBinder<InvalidStateResponse>
    {
        public override async Task BindAsync(InvalidStateResponse responseData, HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status403Forbidden;
            await response.WriteAsync(responseData.Reason);
        }
    }
}