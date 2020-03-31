using System;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
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

        public Task Invoke(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}