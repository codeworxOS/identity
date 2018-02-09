namespace Codeworx.Identity.ContentType
{
    public interface IContentTypeProvider
    {
        bool TryGetContentType(string subpath, out string contentType);
    }
}