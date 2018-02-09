using System;

namespace Codeworx.Identity.Configuration
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class EmbeddedAssetsAttribute : Attribute
    {
        public EmbeddedAssetsAttribute(string uriPrefix, string assetsFolder = "assets")
        {
            AssetsFolder = assetsFolder;
            UriPrefix = uriPrefix;
        }

        public string AssetsFolder { get; }

        public string UriPrefix { get; }
    }
}