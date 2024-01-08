using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;

namespace Codeworx.Identity
{
    [DataContract]
    public class IdentityData
    {
        public IdentityData(string clientId, string identifier, string login, IEnumerable<AssignedClaim> claims = null, string externalTokenKey = null)
        {
            ClientId = clientId;
            Identifier = identifier;
            Login = login;
            ExternalTokenKey = externalTokenKey;
            Claims = ImmutableList.CreateRange(claims ?? Enumerable.Empty<AssignedClaim>());
        }

        [DataMember(Order = 5, Name = Constants.Claims.ExternalTokenKey)]
        public string ExternalTokenKey { get; }

        [DataMember(Order = 4, Name = "client_id")]
        public string ClientId { get; }

        [DataMember(Order = 3, Name = "claims")]
        public IEnumerable<AssignedClaim> Claims { get; }

        [DataMember(Order = 2, Name = "identifier")]
        public string Identifier { get; }

        [DataMember(Order = 1, Name = "login")]
        public string Login { get; }

        public IDictionary<string, object> GetTokenClaims(ClaimTarget target)
        {
            var run = 0;
            var claims = this.Claims.Where(claim => claim.Target.HasFlag(target)).ToList();

            Dictionary<string, object> result = GetClaimsStructure(run, claims);

            return result;
        }

        private static Dictionary<string, object> GetClaimsStructure(int run, IEnumerable<AssignedClaim> claims)
        {
            var result = new Dictionary<string, object>();
            foreach (var grp in claims.GroupBy(p => p.Type.ElementAt(run)))
            {
                var children = new List<AssignedClaim>();
                var values = new HashSet<object>();
                foreach (var item in grp.Select(p => p))
                {
                    if (item.Type.Count() == run + 1)
                    {
                        foreach (var value in item.Values)
                        {
                            values.Add(value);
                        }
                    }
                    else
                    {
                        children.Add(item);
                    }
                }

                if (children.Any())
                {
                    values.Add(GetClaimsStructure(run + 1, children));
                }

                result.Add(grp.Key, values.Count > 1 ? (object)values.ToList() : values.FirstOrDefault());
            }

            return result;
        }
    }
}