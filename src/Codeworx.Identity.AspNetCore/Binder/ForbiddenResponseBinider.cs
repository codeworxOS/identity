using System.Threading.Tasks;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class ForbiddenResponseBinider : ResponseBinder<ForbiddenResponse>
    {
        protected override Task BindAsync(ForbiddenResponse responseData, HttpResponse response, bool headerOnly)
        {
            response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
    }
}
