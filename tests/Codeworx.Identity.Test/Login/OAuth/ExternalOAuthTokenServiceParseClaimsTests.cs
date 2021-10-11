using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Cryptography.Internal;
using Codeworx.Identity.Cryptography.Json;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Login.OAuth
{
    public class ExternalOAuthTokenServiceParseClaimsTests
    {

        [Test]
        public async Task TestAllowNullClaimValues()
        {
            var identity = new ClaimsIdentity();

            var jwtToken = new Jwt(new DefaultSigningKeyProvider(), new JwtConfiguration());
            await jwtToken.ParseAsync("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwiY2hpbGQiOnsiY2hpbGQxIjoidmFsdWUiLCJjaGlsZDIiOm51bGx9LCJuYW1lIjpudWxsfQ.70VTkJeGa4FtKIRi0jcum8n_0MvlC5s6YvEMiTwW15Q");

            await ExternalOAuthTokenService.AppendClaimsAsync(jwtToken, identity, "id_token");
        }
    }
}
