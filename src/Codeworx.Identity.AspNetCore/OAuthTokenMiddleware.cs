using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class OAuthTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public OAuthTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await context.Response.WriteAsync("Token endpoint");
            //return this._next(context);
        }
    }
}