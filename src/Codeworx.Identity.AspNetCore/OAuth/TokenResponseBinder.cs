using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class TokenResponseBinder : IResponseBinder
    {
        public bool Supports(Type responseType)
        {
            return responseType == typeof(TokenResponse);
        }

        public async Task RespondAsync(object response, HttpContext context)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!(response is TokenResponse tokenResponse))
            {
                throw new NotSupportedException($"This binder only supports {typeof(TokenResponse)}");
            }

            context.Response.Headers.Add(HeaderNames.ContentType, "application/json;charset=UTF8");
            context.Response.Headers.Add(HeaderNames.CacheControl, "no-store");
            context.Response.Headers.Add(HeaderNames.Pragma, "no-cache");

            var responseString = JsonConvert.SerializeObject(tokenResponse);

            await context.Response.WriteAsync(responseString)
                         .ConfigureAwait(false);
        }
    }
}
