using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public interface IResponseBinder<TResponse> : IResponseBinder
    {
        Task BindAsync(TResponse responseData, HttpResponse response);
    }
}