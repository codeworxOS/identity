using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.AspNetCore
{
    public class EmbeddedResourceMiddleware
    {
        private readonly ImmutableList<IAssetProvider> _assetProviders;
        private readonly RequestDelegate _next;

        public EmbeddedResourceMiddleware(RequestDelegate next, IEnumerable<IAssetProvider> assetProviders)
        {
            _next = next;
            _assetProviders = assetProviders.ToImmutableList();
        }

        public async Task Invoke(HttpContext context, IResponseBinder<AssetResponse> responseBinder)
        {
            var providers = from provider in _assetProviders
                            from prefix in provider.Prefixes
                            orderby prefix.Length descending
                            where context.Request.Path.StartsWithSegments(prefix)
                            select new { Provider = provider, Prefix = prefix };

            foreach (var item in providers)
            {
                if (context.Request.Path.StartsWithSegments(item.Prefix, out var remaining))
                {
                    var assetResponse = await item.Provider.GetAssetAsync(item.Prefix, remaining.Value, context.Request.Headers.IfNoneMatch);
                    {
                        if (assetResponse.FoundAsset)
                        {
                            await responseBinder.BindAsync(assetResponse, context.Response);
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }

        internal static bool Condition(HttpContext ctx)
        {
            var assetProviders = ctx.RequestServices.GetServices<IAssetProvider>();

            return assetProviders.SelectMany(p => p.Prefixes).Any(p => ctx.Request.Path.StartsWithSegments(p));
        }
    }
}