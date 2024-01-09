using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class SchemaInfo
    {
        public SchemaInfo(IEnumerable<SchemaPath> paths, string dataType, Type resourceType)
        {
            Paths = paths.ToImmutableList();
            DataType = dataType;
            ResourceType = resourceType;
        }

        public IReadOnlyList<SchemaPath> Paths { get; }

        public string DataType { get; }

        public Type ResourceType { get; }
    }
}
