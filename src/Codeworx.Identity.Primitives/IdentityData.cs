using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Codeworx.Identity
{
    public class IdentityData
    {
        public IdentityData(string identity, string login, IEnumerable<TenantInfo> tenants, IEnumerable<AssignedClaim> claims, string tenantKey = null)
            : this(identity, login, tenantKey ?? tenants.SingleOrDefault()?.Key, claims)
        {
            Tenants = ImmutableList.CreateRange(tenants);
        }

        private IdentityData(string identifier, string login, string tenantKey, IEnumerable<AssignedClaim> claims)
        {
            Tenants = Enumerable.Empty<TenantInfo>();
            TenantKey = tenantKey;
            Identifier = identifier;
            Login = login;
            Claims = ImmutableList.CreateRange(claims);
        }

        public IEnumerable<AssignedClaim> Claims { get; }

        public string Identifier { get; }

        public string Login { get; }

        public string TenantKey { get; }

        public IEnumerable<TenantInfo> Tenants { get; }
    }
}