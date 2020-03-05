using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public interface IResponseBinder
    {
        Task BindAsync(object responseData, HttpResponse response);
    }
}
