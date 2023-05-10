using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class AssetResponseBinder : ResponseBinder<AssetResponse>
    {
        private readonly IContentTypeLookup _contentTypeLookup;

        public AssetResponseBinder(IContentTypeLookup contentTypeLookup)
        {
            this._contentTypeLookup = contentTypeLookup;
        }

        protected override async Task BindAsync(AssetResponse responseData, HttpResponse response, bool headerOnly)
        {
            if (_contentTypeLookup.TryGetContentType(responseData.Path, out var contentType))
            {
                response.ContentType = contentType;
            }

            if (response.HttpContext.Request.Query.ContainsKey("v"))
            {
                var cacheControl = new CacheControlHeaderValue();
                cacheControl.MaxAge = System.TimeSpan.FromDays(365);
                cacheControl.Extensions.Add(new NameValueHeaderValue("immutable"));
                response.GetTypedHeaders().CacheControl = cacheControl;
            }

            response.StatusCode = StatusCodes.Status200OK;

            if (!headerOnly)
            {
                using (var stream = await responseData.GetAssetStream())
                {
                    response.ContentLength = stream.Length;
                    await stream.CopyToAsync(response.Body);
                }
            }
        }
    }
}