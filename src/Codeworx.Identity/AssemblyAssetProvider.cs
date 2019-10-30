using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public class AssemblyAssetProvider : IAssetProvider
    {
        private readonly Assembly _assembly;
        private readonly IReadOnlyDictionary<string, string> _assets;

        public AssemblyAssetProvider(Assembly assembly)
        {
            _assembly = assembly;
            var result = from a in assembly.GetCustomAttributes<EmbeddedAssetsAttribute>()
                         select new
                         {
                             Prefix = a.UriPrefix,
                             Folder = a.AssetsFolder
                         };

            _assets = result.ToImmutableDictionary(p => p.Prefix, p => p.Folder);
        }

        public IEnumerable<string> Prefixes => _assets.Keys;

        public Task<AssetResponse> GetAssetAsync(string prefix, string path)
        {
            var assetFolderLookup = _assets[prefix];

            var assetFolder = assetFolderLookup.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var reminingPath = path.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            var resourcePath = $"{_assembly.GetName().Name}.{string.Join(".", assetFolder)}.{string.Join(".", reminingPath)}";
            var resourceName = _assembly.GetManifestResourceNames()
                                .SingleOrDefault(p => p.Equals(resourcePath, StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
            {
                return Task.FromResult(AssetResponse.Failed(path));
            }

            return Task.FromResult(AssetResponse.Success(path, () => Task.FromResult(_assembly.GetManifestResourceStream(resourceName))));
        }
    }
}