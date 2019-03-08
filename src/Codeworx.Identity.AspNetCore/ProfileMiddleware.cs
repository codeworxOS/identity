using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Primitives;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Features.Authentication;
using System.IO;
using System.Linq;
using System.Security.Principal;

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

        public async Task Invoke(HttpContext context, AuthenticatedUserInformation authenticatedUserInformation)
        {
            if (authenticatedUserInformation?.IdentityData == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
            
            var data = authenticatedUserInformation.IdentityData;

            if (_service.TryGetContentType(Constants.JsonExtension, out string contentType))
            {
                context.Response.ContentType = contentType;
            }
            context.Response.StatusCode = StatusCodes.Status200OK;

            var setting = new JsonSerializerSettings();
            setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var ser = JsonSerializer.Create(setting);

            using (var sw = new StreamWriter(context.Response.Body))
            {
                using (var jw = new JsonTextWriter(sw))
                {
                    await jw.WriteStartObjectAsync();

                    await jw.WritePropertyNameAsync(Constants.IdClaimType);
                    await jw.WriteValueAsync(data.Identifier);

                    await jw.WritePropertyNameAsync(Constants.NameClaimType);
                    await jw.WriteValueAsync(data.Login);

                    if (data.TenantKey != null)
                    {
                        await jw.WritePropertyNameAsync(Constants.CurrentTenantClaimType);
                        await jw.WriteValueAsync(data.TenantKey);
                    }

                    foreach (var item in data.Claims)
                    {
                        await jw.WritePropertyNameAsync(item.Type);
                        if (item.Values.Count() > 1)
                        {
                            await jw.WriteStartArrayAsync();
                        }

                        foreach (var val in item.Values)
                        {
                            await jw.WriteValueAsync(val);
                        }

                        if (item.Values.Count() > 1)
                        {
                            await jw.WriteEndArrayAsync();
                        }
                    }

                    await jw.WriteEndObjectAsync();
                }
            }
        }
    }
}