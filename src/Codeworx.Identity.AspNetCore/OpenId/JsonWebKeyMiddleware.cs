using System;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class JsonWebKeyMiddleware
    {
        private readonly RequestDelegate _next;

        public JsonWebKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context, IDefaultSigningKeyProvider signingKeyProvider)
        {
            throw new NotImplementedException();
        }
    }
}