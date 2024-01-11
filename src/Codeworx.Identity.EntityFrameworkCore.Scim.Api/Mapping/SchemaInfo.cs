using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class SchemaInfo
    {
        public SchemaInfo(IEnumerable<SchemaPath> paths, string dataType, Type resourceType, string? description, bool isRequired, bool isUnique, bool caseExact, string mutability, List<string>? referenceTypes, List<string>? canonicalValues)
        {
            Paths = paths.ToImmutableList();
            DataType = dataType;
            ResourceType = resourceType;

            Description = description;
            IsRequired = isRequired;
            IsUnique = isUnique;
            CaseExact = caseExact;
            Mutability = mutability;
            ReferenceTypes = referenceTypes;
            CanonicalValues = canonicalValues;
        }

        public IReadOnlyList<SchemaPath> Paths { get; }

        public string DataType { get; }

        public Type ResourceType { get; }

        public string? Description { get; }

        public bool IsRequired { get; }

        public bool IsUnique { get; }

        public bool CaseExact { get; }

        public string Mutability { get; }

        public List<string>? ReferenceTypes { get; }

        public List<string>? CanonicalValues { get; }
    }
}
