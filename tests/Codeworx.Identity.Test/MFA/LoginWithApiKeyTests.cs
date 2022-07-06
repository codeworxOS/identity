using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class LoginWithApiKeyTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task LoginWithApiKey_NoMfaRequired_DoesNotShowMfa()
        {
            await this.ConfigureApiKeySetup(isMfaRequiredOnUser: false, isMfaRequiredOnClient: false);
            var tokenResponse = await this.GetTokenFromApiKey(TestConstants.Clients.MfaTestServiceAccountClientId, TestConstants.Clients.MfaTestServiceAccountClientSecret, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreNotEqual(HttpStatusCode.Redirect, tokenResponse.StatusCode);
            Assert.AreNotEqual(this.GetMfaUrl(), tokenResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task LoginWithApiKey_MfaRequiredOnUser_DoesNotShowMfa()
        {
            await this.ConfigureApiKeySetup(isMfaRequiredOnUser: true, isMfaRequiredOnClient: false);
            var tokenResponse = await this.GetTokenFromApiKey(TestConstants.Clients.MfaTestServiceAccountClientId, TestConstants.Clients.MfaTestServiceAccountClientSecret, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreNotEqual(HttpStatusCode.Redirect, tokenResponse.StatusCode);
            Assert.AreNotEqual(this.GetMfaUrl(), tokenResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task LoginWithApiKey_MfaRequiredOnTenant_DoesNotShowMfa()
        {
            await this.ConfigureApiKeySetup(isMfaRequiredOnUser: false, isMfaRequiredOnClient: false);
            var tokenResponse = await this.GetTokenFromApiKey(TestConstants.Clients.MfaTestServiceAccountClientId, TestConstants.Clients.MfaTestServiceAccountClientSecret, TestConstants.Tenants.MfaTenant.Id);

            Assert.AreNotEqual(HttpStatusCode.Redirect, tokenResponse.StatusCode);
            Assert.AreNotEqual(this.GetMfaUrl(), tokenResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task LoginWithApiKey_MfaRequiredOnClient_DoesNotShowMfa()
        {
            await this.ConfigureApiKeySetup(isMfaRequiredOnUser: false, isMfaRequiredOnClient: true);
            var tokenResponse = await this.GetTokenFromApiKey(TestConstants.Clients.MfaTestServiceAccountClientId, TestConstants.Clients.MfaTestServiceAccountClientSecret, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreNotEqual(HttpStatusCode.Redirect, tokenResponse.StatusCode);
            Assert.AreNotEqual(this.GetMfaUrl(), tokenResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));
        }
    }
}
