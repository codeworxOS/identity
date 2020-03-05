using System.IO;
using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class ProviderInfosResponseBinder : ResponseBinder<ProviderInfosResponse>
    {
        private readonly IContentTypeLookup _lookup;

        public ProviderInfosResponseBinder(IContentTypeLookup lookup)
        {
            _lookup = lookup;
        }

        public override Task BindAsync(ProviderInfosResponse responseData, HttpResponse response)
        {
            if (_lookup.TryGetContentType(Constants.JsonExtension, out string contentType))
            {
                response.ContentType = contentType;
            }

            response.StatusCode = StatusCodes.Status200OK;

            var setting = new JsonSerializerSettings();
            setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var ser = JsonSerializer.Create(setting);

            using (var sw = new StreamWriter(response.Body))
            {
                ser.Serialize(sw, responseData.Providers);
            }

            return Task.CompletedTask;
        }
    }
}