using System;

namespace Codeworx.Identity
{
    public static class CodeworxIdentityPrimitivesBAseUriAccessorIExtensions
    {
        [Obsolete("Ues IBaseUriAccessor.IsValidRelative instead", true)]
        public static bool IsRelative(this IBaseUriAccessor baseUriAccessor, string uri)
        {
            return baseUriAccessor.IsRelative(uri);
        }
    }
}
