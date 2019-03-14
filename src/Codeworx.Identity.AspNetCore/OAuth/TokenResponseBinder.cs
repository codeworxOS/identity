using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class TokenResponseBinder : IResponseBinder<TokenResponse>
    {
        public async Task RespondAsync(TokenResponse response, HttpContext context)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            context.Response.Headers.Add(HeaderNames.ContentType, "application/json;charset=UTF8");
            context.Response.Headers.Add(HeaderNames.CacheControl, "no-store");
            context.Response.Headers.Add(HeaderNames.Pragma, "no-cache");

            var responseString = JsonConvert.SerializeObject(response);

            await context.Response.WriteAsync(responseString)
                         .ConfigureAwait(false);
        }
    }
}
