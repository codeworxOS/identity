using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class TokenResponseBinder : ResponseBinder<TokenResponse>
    {
        public override async Task BindAsync(TokenResponse responseData, HttpResponse response)
        {
            if (responseData == null)
            {
                throw new ArgumentNullException(nameof(responseData));
            }

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            response.Headers.Add(HeaderNames.ContentType, "application/json;charset=UTF8");
            response.Headers.Add(HeaderNames.CacheControl, "no-store");
            response.Headers.Add(HeaderNames.Pragma, "no-cache");

            var responseString = JsonConvert.SerializeObject(responseData);

            await response.WriteAsync(responseString)
                         .ConfigureAwait(false);
        }
    }
}