using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class JsonWebKeyMiddleware
    {
        private readonly RequestDelegate _next;

        public JsonWebKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IDefaultSigningKeyProvider signingKeyProvider)
        {
            var keyParameter = signingKeyProvider.GetKeyParameter();

            context.Response.Headers.Add(HeaderNames.ContentType, "application/json;charset=utf-8");

            var responseString = JsonConvert.SerializeObject(keyParameter);

            await context.Response.WriteAsync(responseString)
                .ConfigureAwait(false);
        }
    }
}