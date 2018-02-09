using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Codeworx.Identity
{
    public static class CodeworxIdenittyIdentityDataExtensions
    {
        public static string GetClaimTargetUrl(this ClaimTarget target)
        {
            return $"{Constants.ClaimTargetUrl}/{target}".ToLower();
        }

        public static string GetIssureUrl(this AssignedClaim.AssignmentSource source)
        {
            return $"{Constants.ClaimSourceUrl}/{source}".ToLower();
        }

        public static IEnumerable<Claim> ToClaims(this AssignedClaim assignedClaim)
        {
            var type = assignedClaim.Type;
            var issuer = assignedClaim.Source.GetIssureUrl();
            var target = assignedClaim.Target.GetClaimTargetUrl();

            foreach (var value in assignedClaim.Values)
            {
                var claim = new Claim(type, value, null, issuer, target);
                yield return claim;
            }
        }

        public static ClaimsPrincipal ToClaimsPrincipal(this IdentityData data)
        {
            var identity = new ClaimsIdentity(Constants.ProductName);
            var userClaims = data.Claims.SelectMany(p => p.ToClaims());
            identity.AddClaims(userClaims);
            var tenants = data.ToTenantClaims();
            identity.AddClaims(tenants);
            var identityClaims = data.ToIdentityClaims();
            identity.AddClaims(identityClaims);

            var principal = new ClaimsPrincipal(identity);

            return principal;
        }

        public static IEnumerable<Claim> ToIdentityClaims(this IdentityData data)
        {
            yield return new Claim(ClaimTypes.NameIdentifier, data.Identifyer);
            yield return new Claim(ClaimTypes.Name, data.Login);
        }

        public static IEnumerable<Claim> ToTenantClaims(this IdentityData data)
        {
            foreach (var item in data.Tenants)
            {
                var type = item.Key == data.TenantKey ? Constants.CurrentTenantClaimType : Constants.TenantClaimType;
                var value = item.Key;

                yield return new Claim(type, value, item.Name);
            }
        }
    }
}