namespace Codeworx.Identity.ContentType
{
    public interface IContentTypeLookup
    {
        bool TryGetContentType(string subPath, out string contentType);
    }
}
