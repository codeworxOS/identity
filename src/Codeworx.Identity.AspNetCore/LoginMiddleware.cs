using System;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Codeworx.Identity.AspNetCore
{
    public class LoginMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Configuration.IdentityService _service;
        private readonly IViewTemplate _template;

        public LoginMiddleware(RequestDelegate next, Configuration.IdentityService service, IViewTemplate template)
        {
            _next = next;
            _service = service;
            _template = template;
        }

        public async Task Invoke(HttpContext context)
        {
            string body = null;

            var result = await context.AuthenticateAsync();

            var hasReturnUrl = context.Request.Query.TryGetValue("returnurl", out StringValues returnUrl);

            if (result.Succeeded)
            {
                body = await _template.GetLoggedInTemplate(returnUrl);
            }
            else
            {
                // TODO var missingTenant = context.AuthenticateAsync(Constants.MissingTenant....);
                // TODO missingTenant.Success -> new Template Select Tenant.

                body = await _template.GetLoginTemplate(returnUrl);
            }

            if (context.Request.Method.Equals(HttpMethods.Post, StringComparison.OrdinalIgnoreCase))
            {
                var setting = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var request = await context.Request.BindAsync<LoginRequest>(setting);
                var userName = request.UserName;
                try
                {
                    var identityProvider = context.RequestServices.GetService<IIdentityService>();
                    var identityData = await identityProvider.LoginAsync(request.UserName, request.Password);
                    var principal = identityData.ToClaimsPrincipal();

                    if (identityData.TenantKey != null)
                    {
                        var authProperties = new AuthenticationProperties();

                        //var test = false;

                        //if (test)
                        //{
                        //    authProperties.IsPersistent = true;
                        //    authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1);
                        //}

                        await context.SignInAsync(_service.AuthenticationScheme, principal, authProperties);
                    }
                    else
                    {
                        await context.SignInAsync(Constants.MissingTenantAuthenticationScheme, principal);
                        context.Response.Redirect(_service.Options.OauthEndpoint);
                        return;
                    }

                    if (hasReturnUrl)
                    {
                        context.Response.Redirect(returnUrl);
                        return;
                    }
                    body = await _template.GetLoggedInTemplate(returnUrl);
                }
                catch (AuthenticationException)
                {
                }
            }

            if (_service.TryGetContentType(".html", out string contentType))
            {
                context.Response.ContentType = contentType;
            }

            await context.Response.WriteAsync(body);

            return;
        }
    }
}