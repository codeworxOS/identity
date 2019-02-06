using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Codeworx.Identity.ContentType;

namespace Codeworx.Identity.Configuration
{
    public class IdentityService
    {
        internal IdentityService(
            string authenticationScheme,
            IdentityOptions options,
            IEnumerable<Assembly> parts,
            IEnumerable<IContentTypeProvider> contentTypeProviders,
            bool windowsAuthentication)
        {
            AuthenticationScheme = authenticationScheme;
            Options = options;
            ContentTypeProviders = contentTypeProviders;
            Parts = ImmutableList.CreateRange(parts);

            var result = from p in parts
                         from a in p.GetCustomAttributes<EmbeddedAssetsAttribute>()
                         group new { a, p } by a.UriPrefix into grp
                         select new
                         {
                             Prefix = grp.Key,
                             Info = grp.Select(x => new
                             {
                                 AssetsFolder = x.a.AssetsFolder,
                                 Assembly = x.p
                             }).ToList()
                         };

            var first = result.FirstOrDefault(p => p.Info.Count > 1);
            if (first != null)
            {
                throw new ArgumentException($"Multiple EmbeddedAssets registered with the same PrefixUri ({first.Prefix}).", nameof(parts));
            }

            Assets = result.ToImmutableDictionary(p => p.Prefix, p => new AssemblyAsset(p.Info.First().Assembly, p.Info.First().AssetsFolder));
            WindowsAuthentication = windowsAuthentication;
        }

        public ImmutableDictionary<string, AssemblyAsset> Assets { get; }

        public string AuthenticationScheme { get; }

        public IEnumerable<IContentTypeProvider> ContentTypeProviders { get; }

        public IdentityOptions Options { get; }

        public ImmutableList<Assembly> Parts { get; }

        public bool WindowsAuthentication { get; }

        public bool TryGetContentType(string subPath, out string contentType)
        {
            foreach (var item in this.ContentTypeProviders)
            {
                if (item.TryGetContentType(subPath, out contentType))
                {
                    return true;
                }
            }

            contentType = null;
            return false;
        }

        public class AssemblyAsset
        {
            public AssemblyAsset(Assembly assembly, string assetFolder)
            {
                Assembly = assembly;
                AssetFolder = assetFolder;
            }

            public Assembly Assembly { get; }

            public string AssetFolder { get; }
        }
    }
}