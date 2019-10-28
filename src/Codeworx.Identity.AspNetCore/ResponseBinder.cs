using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public abstract class ResponseBinder<TResponse> : IResponseBinder<TResponse>, IResponseBinder
    {
        public abstract Task BindAsync(TResponse responseData, HttpResponse response);

        Task IResponseBinder.BindAsync(object responseData, HttpResponse response)
        {
            return BindAsync((TResponse)responseData, response);
        }
    }
}
