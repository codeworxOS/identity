using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Primitives;
using Codeworx.Identity.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;

namespace Codeworx.Identity.Mvc
{
    public class EmbeddedResourceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IdentityService _service;

        public EmbeddedResourceMiddleware(RequestDelegate next, IdentityService service)
        {
            _next = next;
            _service = service;
        }

        public async Task Invoke(HttpContext context)
        {
            var prefix = _service.Assets.Keys.OrderByDescending(p => p.Length).FirstOrDefault(p => context.Request.Path.StartsWithSegments(p));

            IdentityService.AssemblyAsset asset;
            if (_service.Assets.TryGetValue(prefix, out asset))
            {
                PathString remaining;
                if (context.Request.Path.StartsWithSegments(prefix, out remaining))
                {
                    var assetFolder = asset.AssetFolder.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    var reminingPath = remaining.Value.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

                    var path = $"{asset.Assembly.GetName().Name}.{string.Join(".", assetFolder)}.{string.Join(".", reminingPath)}";
                    var resourceName = asset.Assembly.GetManifestResourceNames()
                                        .SingleOrDefault(p => p.Equals(path, StringComparison.OrdinalIgnoreCase));

                    if (!string.IsNullOrWhiteSpace(resourceName))
                    {
                        using (var stream = asset.Assembly.GetManifestResourceStream(resourceName))
                        {
                            string contentType;
                            if (_service.TryGetContentType(remaining, out contentType))
                            {
                                context.Response.ContentType = contentType;
                            }

                            context.Response.ContentLength = stream.Length;
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            await stream.CopyToAsync(context.Response.Body);
                        }
                        return;
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return;
                    }
                }
            }

            await _next(context);
        }

        internal static bool Condition(HttpContext ctx)
        {
            var service = ctx.RequestServices.GetRequiredService<IdentityService>();

            return service.Assets.Keys.Any(p => ctx.Request.Path.StartsWithSegments(p));
        }
    }
}