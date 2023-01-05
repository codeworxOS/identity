using System.IO;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public class IntrospectResponseBinder : ResponseBinder<IIntrospectResponse>
    {
        private readonly IContentTypeLookup _lookup;

        public IntrospectResponseBinder(IContentTypeLookup lookup)
        {
            _lookup = lookup;
        }

        protected override async Task BindAsync(IIntrospectResponse responseData, HttpResponse response, bool headerOnly)
        {
            if (_lookup.TryGetContentType(Constants.JsonExtension, out string contentType))
            {
                response.ContentType = contentType;
            }

            response.StatusCode = StatusCodes.Status200OK;

            var setting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var ser = JsonSerializer.Create(setting);

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, 4096, true))
                {
                    ser.Serialize(writer, responseData);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(response.Body).ConfigureAwait(false);
            }
        }
    }
}
