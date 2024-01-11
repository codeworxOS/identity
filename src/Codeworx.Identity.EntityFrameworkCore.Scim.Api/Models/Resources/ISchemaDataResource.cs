using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public interface ISchemaDataResource
    {
        string Id { get; }

        string Name { get; }

        string? Description { get; }

        public List<SchemaDataAttributeResource> Attributes { get; }
    }
}