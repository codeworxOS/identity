using System.Threading.Tasks;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class UnsupportedMediaTypeResponseBinder : ResponseBinder<UnsupportedMediaTypeResponse>
    {
        protected override Task BindAsync(UnsupportedMediaTypeResponse responseData, HttpResponse response, bool headerOnly)
        {
            response.StatusCode = StatusCodes.Status415UnsupportedMediaType;

            return Task.CompletedTask;
        }
    }
}