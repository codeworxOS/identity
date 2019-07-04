using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace Codeworx.Identity.AspNetCore
{
    public class ProfileMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Configuration.IdentityService _service;

        public ProfileMiddleware(RequestDelegate next, Configuration.IdentityService service)
        {
            _next = next;
            _service = service;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var data = ((ClaimsIdentity)context.User.Identity).ToIdentityData();

            if (_service.TryGetContentType(Constants.JsonExtension, out var contentType))
            {
                context.Response.ContentType = contentType;
            }
            context.Response.StatusCode = StatusCodes.Status200OK;

            var setting = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var ser = JsonSerializer.Create(setting);

            using (var streamWriter = new StreamWriter(context.Response.Body))
            {
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    await jsonTextWriter.WriteStartObjectAsync();

                    await jsonTextWriter.WritePropertyNameAsync(Constants.IdClaimType);
                    await jsonTextWriter.WriteValueAsync(data.Identifier);

                    await jsonTextWriter.WritePropertyNameAsync(Constants.NameClaimType);
                    await jsonTextWriter.WriteValueAsync(data.Login);

                    if (data.TenantKey != null)
                    {
                        await jsonTextWriter.WritePropertyNameAsync(Constants.CurrentTenantClaimType);
                        await jsonTextWriter.WriteValueAsync(data.TenantKey);
                    }

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