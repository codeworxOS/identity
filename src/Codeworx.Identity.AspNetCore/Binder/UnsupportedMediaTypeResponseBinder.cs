using System.Threading.Tasks;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class UnsupportedMediaTypeResponseBinder : ResponseBinder<UnsupportedMediaTypeResponse>
    {
        public override Task BindAsync(UnsupportedMediaTypeResponse responseData, HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status415UnsupportedMediaType;

            return Task.CompletedTask;
        }
    }
}