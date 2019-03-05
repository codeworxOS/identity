using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRequestBinder<AuthorizationCodeTokenRequest, object> _tokenRequestBinder;

        public TokenMiddleware(RequestDelegate next, IRequestBinder<AuthorizationCodeTokenRequest, object> tokenRequestBinder)
        {
            _next = next;
            _tokenRequestBinder = tokenRequestBinder;
        }

        public async Task Invoke(HttpContext context)
        {
            var bindingResult = _tokenRequestBinder.FromQuery(context.Request.Form.ToDictionary(p => p.Key, p => p.Value as IReadOnlyCollection<string>));

            await context.Response.WriteAsync("Token endpoint");
            //return this._next(context);
        }
    }
}