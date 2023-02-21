using System;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public class UserInfoResponseBinder : ResponseBinder<UserInfoResponse>
    {
        protected override async Task BindAsync(UserInfoResponse responseData, HttpResponse response, bool headerOnly)
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
                var setting = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };

                var responseString = JsonConvert.SerializeObject(responseData, setting);

                await response.WriteAsync(responseString)
                    .ConfigureAwait(false);
            }
        }
    }
}