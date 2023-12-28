using System;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models
{
    public class ScimResponseInfo
    {
        public ScimResponseInfo(string id, string location, DateTime? created, DateTime? lastModified)
        {
            Id = id;
            Location = location;
            Created = created;
            LastModified = lastModified;
        }

        public string Id { get; set; }

        public string? ExternalId { get; set; }

        public string Location { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? LastModified { get; set; }
    }
}