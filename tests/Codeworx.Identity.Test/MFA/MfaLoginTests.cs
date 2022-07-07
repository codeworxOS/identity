using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class MfaLoginTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task LoginWithCodeFlow_NoMfaRequired_DoesNotShowMfa()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual(this.GetRedirectUrl(), authorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnUser_RedirectsToMfa()
        {
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUserWithMfaRequired.UserName, TestConstants.Users.MfaTestUserWithMfaRequired.Password);

            Assert.AreEqual(HttpStatusCode.Redirect, authenticationResponse.StatusCode);
            Assert.AreEqual(this.GetMfaUrl(), authenticationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnTenant_RedirectsToMfa()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.MfaTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual(this.GetMfaUrl(), authorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnClient_RedirectsToMfa()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.MfaRequiredClientId, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual(this.GetMfaUrl(), authorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
        }
    }
}
