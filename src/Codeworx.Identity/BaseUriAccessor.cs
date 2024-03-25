using System;
using Codeworx.Identity.Configuration;

namespace Codeworx.Identity
{
    public abstract class BaseUriAccessor : IBaseUriAccessor
    {
        private readonly IdentityServerOptions _options;

        protected BaseUriAccessor(IdentityServerOptions options)
        {
            _options = options;
        }

        public abstract Uri BaseUri { get; }

        public bool IsValidRelative(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                return false;
            }

            var toCheck = new Uri(url, UriKind.RelativeOrAbsolute);

            if (toCheck.IsAbsoluteUri)
            {
                return BaseUri.IsBaseOf(toCheck);
            }

            foreach (var item in _options.GetWhitelist(BaseUri))
            {
                if (item.IsBaseOf(toCheck))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
