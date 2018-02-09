using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Codeworx.Identity
{
    public class AssignedClaim
    {
        public AssignedClaim(string type, string value, ClaimTarget target = ClaimTarget.AccessToken, AssignmentSource source = AssignmentSource.System)
            : this(type, new[] { value }, target, source)
        {
        }

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

        public AssignmentSource Source { get; }

        public ClaimTarget Target { get; }

        public string Type { get; }

        public IEnumerable<string> Values { get; }
    }
}