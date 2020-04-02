using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class ErrorResponseBinder : ResponseBinder<ErrorResponse>
    {
        public override async Task BindAsync(ErrorResponse responseData, HttpResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (responseData == null)
            {
                throw new ArgumentNullException(nameof(responseData));
            }

            if (responseData.Error == Identity.OAuth.Constants.Error.InvalidClient)
            {
                response.StatusCode = StatusCodes.Status401Unauthorized;

                if (AuthenticationHeaderValue.TryParse(response.HttpContext.Request.Headers[HeaderNames.Authorization], out var authenticationHeaderValue))
                {
                    response.Headers.Add(HeaderNames.WWWAuthenticate, authenticationHeaderValue.Scheme);
                }
            }
            else
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
            }

            response.Headers.Add(HeaderNames.ContentType, "application/json;charset=utf-8");
            response.Headers.Add(HeaderNames.CacheControl, "no-store");
            response.Headers.Add(HeaderNames.Pragma, "no-cache");

            var responseString = JsonConvert.SerializeObject(responseData);

            await response.WriteAsync(responseString)
                         .ConfigureAwait(false);
        }

        public bool Supports(Type responseType)
        {
            return responseType == typeof(ErrorResponse);
        }
    }
}