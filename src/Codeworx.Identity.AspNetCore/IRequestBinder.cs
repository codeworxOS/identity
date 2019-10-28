using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public interface IRequestBinder<TRequest>
    {
        Task<TRequest> BindAsync(HttpRequest request);
    }
}
