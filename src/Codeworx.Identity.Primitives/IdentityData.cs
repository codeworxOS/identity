using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Codeworx.Identity
{
    public class IdentityData
    {
        public IdentityData(string identifier, string login, IEnumerable<TenantInfo> tenants, IEnumerable<AssignedClaim> claims = null, string tenantKey = null)
            : this(identifier, login, tenantKey, claims ?? Enumerable.Empty<AssignedClaim>())
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

        public IDictionary<string, object> GetTokenClaims(ClaimTarget target)
        {
            var claims = from p in this.Claims
                         where p.Target.HasFlag(target)
                         group p by p.Type into grp
                         let values = grp.SelectMany(x => x.Values).Distinct().ToArray()
                         select new
                         {
                             Key = grp.Key,
                             Value = values.Length > 1 ? (object)values : values.FirstOrDefault()
                         };

            var result = claims.ToDictionary(p => p.Key, p => p.Value);

            result[Constants.Claims.Id] = Identifier;
            result[Constants.Claims.Name] = Login;
            result[Constants.Claims.CurrentTenant] = TenantKey;
            result[Constants.Claims.Tenant] = Tenants.ToDictionary(p => p.Key, p => (object)p.Name);

            return result;
        }
    }
}