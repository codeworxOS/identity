using System;
using System.IO;
using System.Threading.Tasks;

namespace Codeworx.Identity.Model
{
    public class AssetResponse
    {
        private readonly Func<Task<Stream>> _getAssetStream;

        private AssetResponse(string path, Func<Task<Stream>> getAssetStream)
        {
            if (getAssetStream == null)
            {
                throw new ArgumentNullException(nameof(getAssetStream));
            }

            FoundAsset = true;
            Path = path;
            _getAssetStream = getAssetStream;
        }

        private AssetResponse(string path)
        {
            FoundAsset = false;
            Path = path;
        }

        public bool FoundAsset { get; }

        public string Path { get; }

        public static AssetResponse Failed(string path)
        {
            return new AssetResponse(path);
        }

        public static AssetResponse Success(string path, Func<Task<Stream>> getAssetStream)
        {
            return new AssetResponse(path, getAssetStream);
        }

        public async Task<Stream> GetAssetStream()
        {
            if (!FoundAsset)
            {
                throw new NotSupportedException("You can only get the asset stream if th asset is found!");
            }

            return await _getAssetStream();
        }
    }
}