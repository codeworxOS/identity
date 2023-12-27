namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources
{
    public interface ICommonResponseResource : ICommonRequestResource
    {
        ScimMetadata Meta { get; }

        string Id { get; }
    }
}