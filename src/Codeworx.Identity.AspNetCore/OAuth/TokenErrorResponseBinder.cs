using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class TokenErrorResponseBinder : IResponseBinder<TokenErrorResponse>
    {
        public async Task RespondAsync(TokenErrorResponse response, HttpContext context)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (response.Error == Identity.OAuth.Constants.Error.InvalidClient)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                if (AuthenticationHeaderValue.TryParse(context.Request.Headers[HeaderNames.Authorization], out var authenticationHeaderValue))
                {
                    context.Response.Headers.Add(HeaderNames.WWWAuthenticate, authenticationHeaderValue.Scheme);
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            
            context.Response.Headers.Add(HeaderNames.ContentType, "application/json;charset=UTF8");
            context.Response.Headers.Add(HeaderNames.CacheControl, "no-store");
            context.Response.Headers.Add(HeaderNames.Pragma, "no-cache");

            var responseString = JsonConvert.SerializeObject(response);

            await context.Response.WriteAsync(responseString)
                         .ConfigureAwait(false);
        }
    }
}
