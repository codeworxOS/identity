using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class QueryParameter
    {
        public QueryParameter(BooleanFilterNode? filter, string? excludedAttributes, Guid providerId)
        {
            ExcludedAttributes = (excludedAttributes ?? string.Empty).Split(",").Select(p => p.Trim()).ToImmutableList();
            Filter = filter;
            ProviderId = providerId;
        }

        public BooleanFilterNode? Filter { get; }

        public Guid ProviderId { get; }

        public IReadOnlyList<string> ExcludedAttributes { get; }
    }
}
