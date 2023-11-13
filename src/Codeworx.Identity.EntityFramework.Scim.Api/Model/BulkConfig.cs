namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model
{
    public class BulkConfig
    {
        public bool Supported { get; set; }

        public int MaxOperations { get; set; }

        public int MaxPayloadSize { get; set; }
    }
}