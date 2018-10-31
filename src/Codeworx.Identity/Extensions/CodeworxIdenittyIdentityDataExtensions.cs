using System;
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

        public static string GetIssuerUrl(this AssignedClaim.AssignmentSource source)
        {
            return $"{Constants.ClaimSourceUrl}/{source}".ToLower();
        }

        public static IEnumerable<AssignedClaim> ToAssignedClaims(this IEnumerable<Claim> claims)
        {
            foreach (var item in claims.GroupBy(p => p.Type))
            {
                var claim = item.First();
                var type = claim.Type;

                yield return new AssignedClaim(type, item.Select(p => p.Value), ParseClaimTarget(claim.Issuer), ParseClaimSource(claim.OriginalIssuer));
            }
        }

        public static IEnumerable<Claim> ToClaims(this AssignedClaim assignedClaim)
        {
            var type = assignedClaim.Type;
            var issuer = assignedClaim.Source.GetIssuerUrl();
            var target = assignedClaim.Target.GetClaimTargetUrl();

            foreach (var value in assignedClaim.Values)
            {
                var claim = new Claim(type, value, null, issuer, target);
                yield return claim;
            }
        }

        public static ClaimsPrincipal ToClaimsPrincipal(this IdentityData data)
        {
            var identity = new ClaimsIdentity(Constants.ProductName, Constants.NameClaimType, Constants.RoleClaimType);

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
            yield return new Claim(Constants.IdClaimType, data.Identifyer);
            yield return new Claim(Constants.NameClaimType, data.Login);
        }

        public static IdentityData ToIdentityData(this ClaimsIdentity identity)
        {
            string id = null;
            string login = null;
            string currentTenant = null;
            List<TenantInfo> tenants = new List<TenantInfo>();
            List<Claim> claims = new List<Claim>();

            foreach (var item in identity.Claims)
            {
                switch (item.Type)
                {
                    case Constants.IdClaimType:
                        id = item.Value;
                        break;

                    case Constants.NameClaimType:
                        login = item.Value;
                        break;

                    case Constants.CurrentTenantClaimType:
                        currentTenant = item.Value;
                        tenants.Add(new TenantInfo { Key = currentTenant, Name = item.Properties[Constants.TenantNameProperty] });
                        break;

                    case Constants.TenantClaimType:
                        tenants.Add(new TenantInfo { Key = item.Value, Name = item.Properties[Constants.TenantNameProperty] });
                        break;

                    default:
                        claims.Add(item);
                        break;
                }
            }

            return new IdentityData(id, login, tenants, claims.ToAssignedClaims(), currentTenant);
        }

        public static IEnumerable<Claim> ToTenantClaims(this IdentityData data)
        {
            foreach (var item in data.Tenants)
            {
                var type = item.Key == data.TenantKey ? Constants.CurrentTenantClaimType : Constants.TenantClaimType;
                var value = item.Key;

                var claim = new Claim(type, value, item.Name);
                claim.Properties.Add(Constants.TenantNameProperty, item.Name);
                yield return claim;
            }
        }

        private static AssignedClaim.AssignmentSource ParseClaimSource(string issuer)
        {
            var target = issuer.Substring(Constants.ClaimSourceUrl.Length);
            return (AssignedClaim.AssignmentSource)Enum.Parse(typeof(AssignedClaim.AssignmentSource), target, true);
        }

        private static ClaimTarget ParseClaimTarget(string originalIssuer)
        {
            var target = originalIssuer.Substring(Constants.ClaimTargetUrl.Length);
            return (ClaimTarget)Enum.Parse(typeof(ClaimTarget), target, true);
        }
    }
}