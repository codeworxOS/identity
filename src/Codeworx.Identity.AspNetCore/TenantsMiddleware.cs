using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.ContentType;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Codeworx.Identity.AspNetCore
{
    public class TenantsMiddleware
    {
        private readonly IContentTypeLookup _contentTypeLookup;
        private readonly RequestDelegate _next;

        public TenantsMiddleware(RequestDelegate next, IContentTypeLookup contentTypeLookup)
        {
            _next = next;
            _contentTypeLookup = contentTypeLookup;
        }

        public async Task Invoke(HttpContext context, IUserService userService, ITenantService tenantService, IOptionsSnapshot<IdentityOptions> options)
        {
            var authenticationResult = await context.AuthenticateAsync(options.Value.MissingTenantAuthenticationScheme);

            if (!authenticationResult.Succeeded)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var identity = (ClaimsIdentity)authenticationResult.Principal.Identity;

            var tenants = await tenantService.GetTenantsByIdentityAsync(identity) ?? Enumerable.Empty<TenantInfo>();

            if (_contentTypeLookup.TryGetContentType(Constants.JsonExtension, out string contentType))
            {
                context.Response.ContentType = contentType;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;

            var setting = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonSerializer.Create(setting);

            using (var responseWriter = new StreamWriter(context.Response.Body))
            {
                json.Serialize(responseWriter, tenants);
            }
        }
    }
}