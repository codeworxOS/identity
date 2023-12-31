using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class SchemaDataAttributeResource
    {
        public SchemaDataAttributeResource(string name, string type, bool multiValued, string? description, bool required, List<string>? canonicalValues, bool caseExact, string? mutability, string? returned, string? uniqueness, List<string>? referenceTypes)
        {
            Name = name;
            Type = type;
            MultiValued = multiValued;
            Description = description;
            Required = required;
            CanonicalValues = canonicalValues;
            CaseExact = caseExact;
            Mutability = mutability;
            Returned = returned;
            Uniqueness = uniqueness;
            ReferenceTypes = referenceTypes;
        }

        public SchemaDataAttributeResource(string name, bool multiValued, string? description, bool required, bool caseExact)
            : this(name, "complex", multiValued, description, required, null, caseExact, null, null, null, null)
        {
            SubAttributes = new List<SchemaDataAttributeResource>();
        }

        public string Name { get; }

        public string Type { get; }

        public bool MultiValued { get; }

        public string? Description { get; }

        public bool Required { get; }

        public List<string>? CanonicalValues { get; }

        public bool CaseExact { get; }

        public string? Mutability { get; }

        public string? Returned { get; }

        public string? Uniqueness { get; }

        public List<string>? ReferenceTypes { get; }

        public List<SchemaDataAttributeResource>? SubAttributes { get; }
    }
}
