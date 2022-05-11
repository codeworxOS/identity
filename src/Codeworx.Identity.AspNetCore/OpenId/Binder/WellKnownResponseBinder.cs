using System;
using System.Threading.Tasks;
using Codeworx.Identity.OpenId.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore.OpenId.Binder
{
    public class WellKnownResponseBinder : ResponseBinder<WellKnownResponse>
    {
        protected override async Task BindAsync(WellKnownResponse responseData, HttpResponse response, bool headerOnly)
        {
            if (responseData == null)
            {
                throw new ArgumentNullException(nameof(responseData));
            }

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            response.Headers.Add(HeaderNames.ContentType, "application/json;charset=utf-8");

            if (headerOnly)
            {
                response.StatusCode = StatusCodes.Status200OK;
            }
            else
            {
                var responseString = JsonConvert.SerializeObject(responseData);

                await response.WriteAsync(responseString)
                             .ConfigureAwait(false);
            }
        }
    }
}