using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.OpenId.Model;
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

        public async Task Invoke(HttpContext context, IEnumerable<IDefaultSigningKeyProvider> keyProviders)
        {
            var keys = new KeyList
            {
                Keys = keyProviders.Select(p => p.GetKeyParameter()).ToArray()
            };

            context.Response.Headers.Add(HeaderNames.ContentType, "application/json;charset=utf-8");

            var responseString = JsonConvert.SerializeObject(keys);

            await context.Response.WriteAsync(responseString)
                .ConfigureAwait(false);
        }

        [DataContract]
        public class KeyList
        {
            [DataMember(Name = "keys")]
            public KeyParameter[] Keys
            {
                get; set;
            }
        }
    }
}