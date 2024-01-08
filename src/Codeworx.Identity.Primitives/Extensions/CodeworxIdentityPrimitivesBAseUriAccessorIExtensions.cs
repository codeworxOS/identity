using System;

namespace Codeworx.Identity
{
    public static class CodeworxIdentityPrimitivesBAseUriAccessorIExtensions
    {
        public static bool IsRelative(this IBaseUriAccessor baseUriAccessor, string uri)
        {
            var toCheck = new Uri(uri, UriKind.RelativeOrAbsolute);

            if (toCheck.IsAbsoluteUri)
            {
                return baseUriAccessor.BaseUri.IsBaseOf(toCheck);
            }

            if (uri.StartsWith("//"))
            {
                return false;
            }

            return true;
        }
    }
}
