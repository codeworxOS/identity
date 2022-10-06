using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class MfaCookieTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task LoginWithCodeFlow_NoMfaRequired_NoCookie()
        {
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            Assert.IsTrue(this.HasLoginCookie(authenticationResponse), "Authentication Login Cookie");
            Assert.IsFalse(this.HasMfaCookie(authenticationResponse), "Authentication MFA Cookie");

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            Assert.IsFalse(this.HasMfaCookie(authorizationResponse), "Authorization MFA Cookie");
            Assert.IsTrue(this.HasCodeParameter(authorizationResponse), "Code Parameter");
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnUser_CookieAfterMfaFulfilled()
        {
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUserWithMfaRequired.UserName, TestConstants.Users.MfaTestUserWithMfaRequired.Password);
            Assert.IsTrue(this.HasLoginCookie(authenticationResponse), "Authentication Login Cookie");
            Assert.IsFalse(this.HasMfaCookie(authenticationResponse), "Authentication MFA Cookie");

            var mfaResponse = await this.FulfillMfa(TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret, authenticationResponse);
            Assert.IsTrue(this.HasMfaCookie(mfaResponse), "MFA Response MFA Cookie");

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            Assert.IsTrue(this.HasCodeParameter(authorizationResponse), "Code Parameter");

        }

        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnTenant_CookieAfterMfaFulfilled()
        {
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            Assert.IsTrue(this.HasLoginCookie(authenticationResponse), "Authentication Login Cookie");
            Assert.IsFalse(this.HasMfaCookie(authenticationResponse), "Authentication MFA Cookie");

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.MfaTenant.Id);
            Assert.IsFalse(this.HasMfaCookie(authorizationResponse), "Authorization MFA Cookie");
            Assert.AreEqual(this.GetMfaUrl(), authorizationResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));

            var mfaResponse = await this.FulfillMfa(TestConstants.Users.MfaTestUser.MfaSharedSecret, authorizationResponse);
            Assert.IsTrue(this.HasMfaCookie(mfaResponse), "MFA Response MFA Cookie");
            Assert.AreEqual(HttpStatusCode.Redirect, mfaResponse.StatusCode);

            authorizationResponse = await this.TestClient.GetAsync(mfaResponse.Headers.Location);

            Assert.IsTrue(this.HasCodeParameter(authorizationResponse), "Code Parameter");
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnClient_CookieAfterMfaFulfilled()
        {
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            Assert.IsTrue(this.HasLoginCookie(authenticationResponse), "Authentication Login Cookie");
            Assert.IsFalse(this.HasMfaCookie(authenticationResponse), "Authentication MFA Cookie");

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.MfaRequiredClientId, TestConstants.Tenants.DefaultTenant.Id);
            Assert.IsFalse(this.HasMfaCookie(authorizationResponse), "Authorization MFA Cookie");

            var mfaResponse = await this.FulfillMfa(TestConstants.Users.MfaTestUser.MfaSharedSecret, authorizationResponse);
            Assert.IsTrue(this.HasMfaCookie(mfaResponse), "MFA Response MFA Cookie");
            Assert.AreEqual(HttpStatusCode.Redirect, mfaResponse.StatusCode);

            authorizationResponse = await this.TestClient.GetAsync(mfaResponse.Headers.Location);

            Assert.IsTrue(this.HasCodeParameter(authorizationResponse), "Code Parameter");
        }
    }
}
