namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Config
{
    public class BulkConfig
    {
        public bool Supported { get; set; }

        public int MaxOperations { get; set; }

        public int MaxPayloadSize { get; set; }
    }
}