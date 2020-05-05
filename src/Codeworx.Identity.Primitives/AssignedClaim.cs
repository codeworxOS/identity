using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace Codeworx.Identity
{
    [DataContract]
    public class AssignedClaim
    {
        public AssignedClaim(IEnumerable<string> type, IEnumerable<string> values, ClaimTarget target = ClaimTarget.AccessToken)
        {
            Target = target;
            Type = ImmutableList.CreateRange(type);
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

        [DataMember(Order = 2, Name = "target")]
        public ClaimTarget Target { get; }

        [DataMember(Order = 3, Name = "type")]
        public IEnumerable<string> Type { get; }

        [DataMember(Order = 4, Name = "values")]
        public IEnumerable<string> Values { get; }

        public static AssignedClaim Create(string type, string value, ClaimTarget target = ClaimTarget.AllTokens)
        {
            return new AssignedClaim(new[] { type }, new[] { value }, target);
        }

        public static AssignedClaim Create(string type, IEnumerable<string> value, ClaimTarget target = ClaimTarget.AllTokens)
        {
            return new AssignedClaim(new[] { type }, value, target);
        }
    }
}