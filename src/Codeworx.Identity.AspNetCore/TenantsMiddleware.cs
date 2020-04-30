using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore
{
    public class TenantsMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IOptionsSnapshot<IdentityOptions> options, IRequestBinder<AuthorizationRequest> authorizationRequestBinder)
        {
            var authenticationResult = await context.AuthenticateAsync(options.Value.AuthenticationScheme);

            if (!authenticationResult.Succeeded)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var request = await authorizationRequestBinder.BindAsync(context.Request);


            ////var identity = (ClaimsIdentity)authenticationResult.Principal.Identity;

            ////var tenants = await tenantService.GetTenantsByIdentityAsync(identity) ?? Enumerable.Empty<TenantInfo>();

            ////if (_contentTypeLookup.TryGetContentType(Constants.JsonExtension, out string contentType))
            ////{
            ////    context.Response.ContentType = contentType;
            ////}

            ////context.Response.StatusCode = StatusCodes.Status200OK;

            ////var setting = new JsonSerializerSettings
            ////{
            ////    ContractResolver = new CamelCasePropertyNamesContractResolver()
            ////};

            ////var json = JsonSerializer.Create(setting);

            ////using (var responseWriter = new StreamWriter(context.Response.Body))
            ////{
            ////    json.Serialize(responseWriter, tenants);
            ////}
        }
    }
}