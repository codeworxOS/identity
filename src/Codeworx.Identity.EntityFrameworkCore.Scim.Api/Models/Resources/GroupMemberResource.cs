﻿using System.Text.Json.Serialization;
using Codeworx.Identity.Configuration;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class GroupMemberResource : MultiValueResource<string>
    {
        [ScimCanonicalValues("User", "Group")]
        [ScimMutability(ScimMutabilityAttribute.MutabilityType.Immutable)]
        public override string? Type { get; set; }

        [ScimReferenceTypes("User", "Group")]
        [ScimMutability(ScimMutabilityAttribute.MutabilityType.Immutable)]
        [JsonPropertyName("$ref")]
        public override string? Ref { get; set; }

        [ScimMutability(ScimMutabilityAttribute.MutabilityType.Immutable)]
        [ScimRequired(false)]
        public override string Value { get => base.Value; set => base.Value = value; }

        [ScimMutability(ScimMutabilityAttribute.MutabilityType.Immutable)]
        [ScimRequired(false)]
        public override string? Display { get => base.Display; set => base.Display = value; }

        [JsonIgnore]
        public override bool Primary { get => base.Primary; set => base.Primary = value; }
    }
}