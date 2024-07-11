using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Test.Provider;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Account.Invitation
{
    public class InvitationReturnUrlValidatorTest : IntegrationTestBase
    {
        [Test]
        public async Task CheckInvitationIdWithReturnUrlOnExternalOauthProviderAsync()
        {
            var response = await this.TestClient.GetAsync($"https://localhost/account/oauth/{TestConstants.LoginProviders.ExternalOAuthProvider.Id}?invitation={TestConstants.Invitations.Default.Code}");
            Assert.AreEqual(HttpStatusCode.Found, response.StatusCode);
            var location = response.Headers.Location;
            var builder = new UriBuilder(location);
            Assert.True(builder.Query.TryGetValue(Constants.OAuth.StateName, out var state));
            await using var scope = TestServer.Services.CreateAsyncScope();
            var cache = scope.ServiceProvider.GetRequiredService<IStateLookupCache>();
            var lookup = await cache.GetAsync(state.FirstOrDefault());
            Assert.AreEqual(TestConstants.Invitations.Default.ReturnUrl, lookup.ReturnUrl);
        }
    }
}
