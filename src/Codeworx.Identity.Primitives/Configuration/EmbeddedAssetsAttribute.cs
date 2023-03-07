using System;

namespace Codeworx.Identity.Configuration
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class EmbeddedAssetsAttribute : Attribute
    {
        public EmbeddedAssetsAttribute(string uriPrefix, string assetsFolder = "assets", string eTag = null)
        {
            AssetsFolder = assetsFolder;
            UriPrefix = uriPrefix;
            ETag = eTag;
        }

        public string AssetsFolder { get; }

        public string UriPrefix { get; }

        public string ETag { get; }
    }
}