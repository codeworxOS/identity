namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model
{
    public class ResourceTypeResponse
    {
        public ResourceTypeResponse()
        {
            Schemas = new string[]
            {
                SchemaConstants.ResourceType,
            };
        }

        public string[] Schemas { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string EndPoint { get; set; }

        public string Schema { get; set; }

        public MetaData Meta { get; set; }
    }
}
