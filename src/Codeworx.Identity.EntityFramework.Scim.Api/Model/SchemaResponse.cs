namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model
{
    public class SchemaResponse
    {
        public SchemaResponse()
        {
            Schemas = new string[]
            {
                SchemaConstants.Schema,
            };
        }

        public string[] Schemas { get; set; }
    }
}
