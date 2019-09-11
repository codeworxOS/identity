using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public interface IResponseBinder
    {
        bool Supports(Type responseType);
        Task RespondAsync(object response, HttpContext context);
    }
}
