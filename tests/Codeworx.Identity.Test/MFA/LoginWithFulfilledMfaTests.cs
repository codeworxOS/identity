using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class LoginWithFulfilledMfaTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task LoginWithCodeFlow_MfaFulfilled_NoMfaRequired_DoesNotShowMfaAfterAuthorization()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            await this.FulfillMfa(TestConstants.Users.MfaTestUser.MfaSharedSecret);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual(this.GetRedirectUrl(), authorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
            Assert.IsTrue(this.HasCodeParameter(authorizationResponse), "Code Parameter");
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaFulfilled_MfaRequiredOnUser_DoesNotShowMfaAfterAuthorization()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUserWithMfaRequired.UserName, TestConstants.Users.MfaTestUserWithMfaRequired.Password);
            await this.FulfillMfa(TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual(this.GetRedirectUrl(), authorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
            Assert.IsTrue(this.HasCodeParameter(authorizationResponse), "Code Parameter");
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaFulfilled_MfaRequiredOnTenant_DoesNotShowMfaAfterAuthorization()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            await this.FulfillMfa(TestConstants.Users.MfaTestUser.MfaSharedSecret);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.MfaTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual(this.GetRedirectUrl(), authorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
            Assert.IsTrue(this.HasCodeParameter(authorizationResponse), "Code Parameter");
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaFulfilled_MfaRequiredOnClient_DoesNotShowMfaAfterAuthorization()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            await this.FulfillMfa(TestConstants.Users.MfaTestUser.MfaSharedSecret);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.MfaRequiredClientId, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual(this.GetRedirectUrl(), authorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
            Assert.IsTrue(this.HasCodeParameter(authorizationResponse), "Code Parameter");
        }
    }
}
