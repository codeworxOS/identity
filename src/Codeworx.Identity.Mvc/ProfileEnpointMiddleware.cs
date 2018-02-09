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

namespace Codeworx.Identity.Mvc
{
    public class ProfileEnpointMiddleware : AuthenticatedMiddleware
    {
        public ProfileEnpointMiddleware(RequestDelegate next, IdentityService service)
            : base(next, service)
        {
        }

        protected override async Task OnInvokeAsync(HttpContext context, IPrincipal principal)
        {
            var ci = principal.Identity as ClaimsIdentity;

            if (Service.TryGetContentType(Constants.JsonExtension, out string contentType))
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

                    foreach (var item in ci.Claims.GroupBy(p => p.Type))
                    {
                        await jw.WritePropertyNameAsync(item.Key);
                        if (item.Count() > 1)
                        {
                            await jw.WriteStartArrayAsync();
                        }

                        foreach (var val in item)
                        {
                            await jw.WriteValueAsync(val.Value);
                        }

                        if (item.Count() > 1)
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