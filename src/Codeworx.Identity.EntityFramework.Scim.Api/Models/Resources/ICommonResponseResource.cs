namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public interface ICommonResponseResource : ICommonRequestResource
    {
        ScimMetadata Meta { get; }

        string Id { get; }
    }
}