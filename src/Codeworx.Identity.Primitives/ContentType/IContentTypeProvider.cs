namespace Codeworx.Identity.ContentType
{
    public interface IContentTypeProvider
    {
        bool TryGetContentType(string subPath, out string contentType);
    }
}