using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.OpenId;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class WellKnownMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IdentityOptions _options;

        public WellKnownMiddleware(RequestDelegate next, IOptions<IdentityOptions> identityOptions)
        {
            _next = next;
            _options = identityOptions.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            var host = $"{context.Request.Scheme}://{context.Request.Host}";

            var content = new WellKnownResponse
            {
                Issuer = host,
                AuthorizationEndpoint = host + _options.OpenIdAuthorizationEndpoint,
                TokenEndpoint = host + _options.OpenIdTokenEndpoint,
                JsonWebKeyEndpoint = host + _options.OpenIdJsonWebKeyEndpoint,
            };

            var responseBinder = context.GetResponseBinder<WellKnownResponse>();
            await responseBinder.BindAsync(content, context.Response);
        }
    }
}