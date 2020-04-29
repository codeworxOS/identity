using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Codeworx.Identity.AspNetCore
{
    public class ProfileMiddleware
    {
        private readonly IContentTypeLookup _contentTypeLookup;
        private readonly RequestDelegate _next;

        public ProfileMiddleware(RequestDelegate next, IContentTypeLookup contentTypeLookup)
        {
            _next = next;
            _contentTypeLookup = contentTypeLookup;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var data = ((ClaimsIdentity)context.User.Identity).ToIdentityData();

            if (_contentTypeLookup.TryGetContentType(Constants.JsonExtension, out var contentType))
            {
                context.Response.ContentType = contentType;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;

            var setting = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
            var ser = JsonSerializer.Create(setting);

            using (var streamWriter = new StreamWriter(context.Response.Body))
            {
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    await jsonTextWriter.WriteStartObjectAsync();

                    await jsonTextWriter.WritePropertyNameAsync(nameof(data.Identifier));
                    await jsonTextWriter.WriteValueAsync(data.Identifier);

                    await jsonTextWriter.WritePropertyNameAsync(nameof(data.Login));
                    await jsonTextWriter.WriteValueAsync(data.Login);

                    foreach (var item in data.Claims)
                    {
                        await jsonTextWriter.WritePropertyNameAsync(item.Type);
                        if (item.Values.Count() > 1)
                        {
                            await jsonTextWriter.WriteStartArrayAsync();
                        }

                        foreach (var value in item.Values)
                        {
                            await jsonTextWriter.WriteValueAsync(value);
                        }

                        if (item.Values.Count() > 1)
                        {
                            await jsonTextWriter.WriteEndArrayAsync();
                        }
                    }

                    await jsonTextWriter.WriteEndObjectAsync();
                }
            }
        }
    }
}