using System.Collections.Generic;

namespace Codeworx.Identity.ContentType
{
    public class ContentTypeLookup : IContentTypeLookup
    {
        private readonly IEnumerable<IContentTypeProvider> _providers;

        public ContentTypeLookup(IEnumerable<IContentTypeProvider> providers)
        {
            this._providers = providers;
        }

        public bool TryGetContentType(string extension, out string contentType)
        {
            foreach (var item in _providers)
            {
                if (item.TryGetContentType(extension, out contentType))
                {
                    return true;
                }
            }

            contentType = null;
            return false;
        }
    }
}
