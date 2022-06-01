using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Cryptography.Internal;
using Codeworx.Identity.Cryptography.Json;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Login.OAuth
{
    public class ExternalOAuthTokenServiceParseClaimsTests
    {

        [Test]
        public async Task TestAllowNullClaimValues()
        {
            var identity = new ClaimsIdentity();

            var optionsMonitorMock = new Mock<IOptionsMonitor<IdentityOptions>>();
            optionsMonitorMock.Setup(o => o.CurrentValue).Returns(new IdentityOptions());

            var jwtToken = new Jwt(new DefaultSigningKeyProvider(optionsMonitorMock.Object), new JwtConfiguration());
            await jwtToken.ParseAsync("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwiY2hpbGQiOnsiY2hpbGQxIjoidmFsdWUiLCJjaGlsZDIiOm51bGx9LCJuYW1lIjpudWxsfQ.70VTkJeGa4FtKIRi0jcum8n_0MvlC5s6YvEMiTwW15Q");

            await ExternalOAuthTokenService.AppendClaimsAsync(jwtToken, identity, "id_token");
        }
    }
}
