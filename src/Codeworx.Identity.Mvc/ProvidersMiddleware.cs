using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using System.Text.Encodings.Web;

namespace Codeworx.Identity.Mvc
{
    public class ProvidersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IdentityService _service;

        public ProvidersMiddleware(RequestDelegate next, IdentityService service, IAuthenticationSchemeProvider schemeProvider)
        {
            _next = next;
            _service = service;
            _schemeProvider = schemeProvider;
        }

        public async Task Invoke(HttpContext context)
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
            var setup = context.RequestServices.GetService<IProviderSetup>();
            var providers = await setup.GetProvidersAsync(userName);

            var result = new List<ExternalProvider>();

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            if (_service.WindowsAuthentication && schemes.Any(p => p.Name == Constants.WindowsAuthenticationSchema))
            {
                result.Add(new ExternalProvider
                {
                    Id = Constants.ExternalWindowsProviderId,
                    Name = Constants.ExternalWindowsProviderName,
                    Url = $"winlogin?returnUrl={UrlEncoder.Default.Encode(returnUrl ?? string.Empty)}"
                });
            }

            result.AddRange(providers);

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
                ser.Serialize(sw, result);
            }
        }
    }
}