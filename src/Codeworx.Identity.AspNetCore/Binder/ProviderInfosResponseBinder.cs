using System.IO;
using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class ProviderInfosResponseBinder : ResponseBinder<RegistrationInfoResponse>
    {
        private readonly IContentTypeLookup _lookup;

        public ProviderInfosResponseBinder(IContentTypeLookup lookup)
        {
            _lookup = lookup;
        }

        protected override Task BindAsync(RegistrationInfoResponse responseData, HttpResponse response, bool headerOnly)
        {
            if (_lookup.TryGetContentType(Constants.JsonExtension, out string contentType))
            {
                response.ContentType = contentType;
            }

            response.StatusCode = StatusCodes.Status200OK;

            if (!headerOnly)
            {
                var setting = new JsonSerializerSettings();
                setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
                var ser = JsonSerializer.Create(setting);

                using (var sw = new StreamWriter(response.Body))
                {
                    ser.Serialize(sw, responseData.Groups);
                }
            }

            return Task.CompletedTask;
        }
    }
}