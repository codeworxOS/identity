namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources
{
    public class CommonResponseResource : CommonRequestResource
    {
        public CommonResponseResource(string id, ScimMetadata meta)
        {
            Meta = meta;
            Id = id;
        }

        public ScimMetadata Meta { get; set; }

        public string Id { get; set; }
    }
}