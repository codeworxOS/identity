using System;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class HttpContextBaseUriAccessor : IBaseUriAccessor
    {
        public HttpContextBaseUriAccessor(IHttpContextAccessor httpContextAccessor = null)
        {
            if (httpContextAccessor == null)
            {
                BaseUri = new Uri("http://localhost/");
            }
            else
            {
                var request = httpContextAccessor.HttpContext.Request;
                var uriBuilder = new UriBuilder(request.Scheme, request.Host.Host);
                if (request.Host.Port.HasValue)
                {
                    uriBuilder.Port = request.Host.Port.Value;
                }

                uriBuilder.AppendPath(request.PathBase);
                BaseUri = new Uri(uriBuilder.ToString().TrimEnd('/'));
            }
        }

        public Uri BaseUri { get; }
    }
}