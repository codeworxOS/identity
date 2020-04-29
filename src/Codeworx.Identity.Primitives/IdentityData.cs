using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Codeworx.Identity
{
    public class IdentityData
    {
        public IdentityData(string identifier, string login, IEnumerable<AssignedClaim> claims = null)
        {
            Identifier = identifier;
            Login = login;
            Claims = ImmutableList.CreateRange(claims ?? Enumerable.Empty<AssignedClaim>());
        }

        public IEnumerable<AssignedClaim> Claims { get; }

        public string Identifier { get; }

        public string Login { get; }

        public IDictionary<string, object> GetTokenClaims(ClaimTarget target)
        {
            var claims = from p in this.Claims
                         where p.Target.HasFlag(target)
                         group p by p.Type into grp
                         let values = grp.SelectMany(x => x.Values).Distinct().ToArray()
                         select new
                         {
                             Key = grp.Key,
                             Value = values.Length > 1 ? (object)values : values.FirstOrDefault(),
                         };

            var result = claims.ToDictionary(p => p.Key, p => p.Value);

            result[Constants.Claims.Id] = Identifier;
            result[Constants.Claims.Name] = Login;

            return result;
        }
    }
}