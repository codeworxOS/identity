using System;
using System.Security.Claims;

namespace Codeworx.Identity
{
    public static class CodeworxIdentityPrimitivesClaimsIdentityExtensions
    {
        public static string GetUserId(this ClaimsIdentity claimsIdentity)
        {
            var userIdClaim = claimsIdentity.FindFirst(Constants.Claims.Id);

            if (userIdClaim == null)
            {
                throw new NotSupportedException($"The provided claimsIdentity does not contain an id claim");
            }

            return userIdClaim.Value;
        }
    }
}
