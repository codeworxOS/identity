using System.Threading.Tasks;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Response
{
    public class NotAcceptableResponseBinder : ResponseBinder<NotAcceptableResponse>
    {
        public override async Task BindAsync(NotAcceptableResponse responseData, HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status406NotAcceptable;
            await response.WriteAsync(responseData.Reason);
        }
    }
}
