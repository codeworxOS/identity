using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Codeworx.Identity.AspNetCore
{
    public class TenantsMiddleware
    {
        private readonly Configuration.IdentityService _identityService;
        private readonly RequestDelegate _next;

        public TenantsMiddleware(RequestDelegate next, Configuration.IdentityService identityService)
        {
            _next = next;
            _identityService = identityService;
        }

        public async Task Invoke(HttpContext context, IUserService userService, ITenantService tenantService)
        {
            var authenticationResult = await context.AuthenticateAsync(Constants.MissingTenantAuthenticationScheme);

            if (!authenticationResult.Succeeded)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var identity = (ClaimsIdentity)authenticationResult.Principal.Identity;

            var tenants = await tenantService.GetTenantsByIdentityAsync(identity) ?? Enumerable.Empty<TenantInfo>();

            if (_identityService.TryGetContentType(Constants.JsonExtension, out string contentType))
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