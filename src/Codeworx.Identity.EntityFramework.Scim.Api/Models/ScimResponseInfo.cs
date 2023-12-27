using System;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models
{
    public class ScimResponseInfo
    {
        public ScimResponseInfo(string id, string baseUri, DateTime? created, DateTime? lastModified)
        {
            Id = id;
            BaseUri = baseUri;
            Created = created;
            LastModified = lastModified;
        }

        public string Id { get; set; }

        public string? ExternalId { get; set; }

        public string BaseUri { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? LastModified { get; set; }
    }
}