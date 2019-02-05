using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public interface IResponseBinder<in TResponse>
    {
        Task RespondAsync(TResponse response, HttpContext context);
    }
}
