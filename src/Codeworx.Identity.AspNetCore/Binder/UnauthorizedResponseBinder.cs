using System.Threading.Tasks;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class UnauthorizedResponseBinder : ResponseBinder<UnauthorizedResponse>
    {
        public override Task BindAsync(UnauthorizedResponse responseData, HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
    }
}