using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class AssetResponseBinder : ResponseBinder<AssetResponse>
    {
        private readonly IContentTypeLookup _contentTypeLookup;

        public AssetResponseBinder(IContentTypeLookup contentTypeLookup)
        {
            this._contentTypeLookup = contentTypeLookup;
        }

        public override async Task BindAsync(AssetResponse responseData, HttpResponse response)
        {
            string contentType;
            if (_contentTypeLookup.TryGetContentType(responseData.Path, out contentType))
            {
                response.ContentType = contentType;
            }

            using (var stream = await responseData.GetAssetStream())
            {
                response.ContentLength = stream.Length;
                response.StatusCode = StatusCodes.Status200OK;
                await stream.CopyToAsync(response.Body);
            }
        }
    }
}