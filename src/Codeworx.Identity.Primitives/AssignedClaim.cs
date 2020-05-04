using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace Codeworx.Identity
{
    [DataContract]
    public class AssignedClaim
    {
        ////public AssignedClaim(string type, string value, ClaimTarget target = ClaimTarget.AccessToken, AssignmentSource source = AssignmentSource.System)
        ////    : this(type, new[] { value }, target, source)
        ////{
        ////}

        public AssignedClaim(string type, IEnumerable<string> values, ClaimTarget target = ClaimTarget.AccessToken, AssignmentSource source = AssignmentSource.System)
        {
            Target = target;
            Source = source;
            Type = type;
            Values = ImmutableList.CreateRange(values);
        }

        [Flags]
        public enum AssignmentSource
        {
            None = 0x00,
            System = 0x01,
            Global = 0x02,
            User = 0x04,
            Tenant = 0x08,
            TenantUser = User | Tenant,
        }

        [DataMember(Order = 1, Name = "source")]
        public AssignmentSource Source { get; }

        [DataMember(Order = 2, Name = "target")]
        public ClaimTarget Target { get; }

        [DataMember(Order = 3, Name = "type")]
        public string Type { get; }

        [DataMember(Order = 4, Name = "values")]
        public IEnumerable<string> Values { get; }
    }
}