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
        private readonly IEnumerable<IJwkInformationSerializer> _jwkInformationSerializers;

        public JsonWebKeyMiddleware(RequestDelegate next, IEnumerable<IJwkInformationSerializer> jwkInformationSerializers)
        {
            _next = next;
            _jwkInformationSerializers = jwkInformationSerializers;
        }

        public async Task Invoke(HttpContext context, IDefaultSigningDataProvider dataProvider)
        {
            var data = await dataProvider.GetSigningDataAsync(context.RequestAborted);

            var defaultKey = data.Key;
            var serializer = _jwkInformationSerializers.First(p => p.Supports(defaultKey));

            context.Response.Headers.Append(HeaderNames.ContentType, "application/json;charset=utf-8");

            var responseString = JsonConvert.SerializeObject(new KeyList { Keys = new[] { serializer.SerializeKeyToJsonWebKey(defaultKey, string.Empty) } });

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