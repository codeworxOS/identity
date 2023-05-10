using System;

namespace Codeworx.Identity.Configuration
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class EmbeddedAssetsAttribute : Attribute
    {
#pragma warning disable SA1305 // Field names should not use Hungarian notation
        public EmbeddedAssetsAttribute(string uriPrefix, string assetsFolder = "assets", string eTag = null)
#pragma warning restore SA1305 // Field names should not use Hungarian notation
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