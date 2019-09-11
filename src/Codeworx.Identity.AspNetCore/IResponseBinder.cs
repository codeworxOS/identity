using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public interface IResponseBinder
    {
        Task RespondAsync(object response, HttpContext context);

        bool Supports(Type responseType);
    }
}