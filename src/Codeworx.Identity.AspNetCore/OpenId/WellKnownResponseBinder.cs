using System;
using System.Threading.Tasks;
using Codeworx.Identity.OpenId;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class WellKnownResponseBinder : ResponseBinder<WellKnownResponse>
    {
        public override async Task BindAsync(WellKnownResponse responseData, HttpResponse response)
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

            var responseString = JsonConvert.SerializeObject(responseData);

            await response.WriteAsync(responseString)
                         .ConfigureAwait(false);
        }
    }
}