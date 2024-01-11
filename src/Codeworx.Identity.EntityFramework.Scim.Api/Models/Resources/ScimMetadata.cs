using System;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class ScimMetadata
    {
        public ScimMetadata(string resourceType, string location, DateTime? created, DateTime? lastModified)
        {
            ResourceType = resourceType;
            Location = location;
            Created = created;
            LastModified = lastModified;
        }

        public string ResourceType { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? LastModified { get; set; }

        public string Location { get; set; }
    }
}