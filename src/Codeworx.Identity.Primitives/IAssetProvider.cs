using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IAssetProvider
    {
        IEnumerable<string> Prefixes { get; }

        Task<AssetResponse> GetAssetAsync(string prefix, string path, string matchHeader);
    }
}