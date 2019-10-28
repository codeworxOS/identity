using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.ExternalLogin;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Codeworx.Identity.AspNetCore
{
    public class ProvidersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IContentTypeLookup _lookup;
        private readonly IAuthenticationSchemeProvider _schemeProvider;

        public ProvidersMiddleware(RequestDelegate next, IContentTypeLookup lookup)
        {
            _next = next;
            _lookup = lookup;
        }

        public async Task Invoke(HttpContext context, IOptionsSnapshot<IdentityOptions> options, IEnumerable<IExternalLoginProvider> externalLogins)
        {
            StringValues userNameValues;
            context.Request.Query.TryGetValue(Constants.UserNameParameterName, out userNameValues);
            StringValues returnUrlValues;
            string returnUrl = null;

            if (context.Request.Query.TryGetValue(Constants.ReturnUrlParameter, out returnUrlValues))
            {
                returnUrl = returnUrlValues.First();
            }

            var userName = userNameValues.FirstOrDefault();

            var result = new List<ExternalProviderInfo>();

            foreach (var item in externalLogins)
            {
                foreach (var externalLogin in await item.GetLoginRegistrationsAsync(userName))
                {
                    var info = new ExternalProviderInfo
                    {
                        Id = externalLogin.Id,
                        Name = externalLogin.Name,
                        Url = "whatever"
                    };
                    result.Add(info);
                }
            }

            if (_lookup.TryGetContentType(Constants.JsonExtension, out string contentType))
            {
                context.Response.ContentType = contentType;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;

            var setting = new JsonSerializerSettings();
            setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var ser = JsonSerializer.Create(setting);

            using (var sw = new StreamWriter(context.Response.Body))
            {
                ser.Serialize(sw, result);
            }
        }
    }
}