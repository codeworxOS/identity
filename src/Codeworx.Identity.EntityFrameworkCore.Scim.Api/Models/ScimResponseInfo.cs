using System;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models
{
    public class ScimResponseInfo : IScimResource
    {
        public ScimResponseInfo(string id, string location, DateTime? created, DateTime? lastModified)
        {
            Id = id;
            Location = location;
            Created = created;
            LastModified = lastModified;
        }

        internal ScimResponseInfo()
        {
            Id = null!;
            Location = null!;
        }

        public string Id { get; set; }

        public string? ExternalId { get; set; }

        public string Location { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? LastModified { get; set; }
    }
}